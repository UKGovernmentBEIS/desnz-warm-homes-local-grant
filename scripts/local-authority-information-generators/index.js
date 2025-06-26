const csv = require('csv');
const { program } = require('commander');
const { readFileSync, writeFileSync } = require('fs');

program.option('--outfile <string>', 'outfile', 'out.txt');
program.option('--mode <string>', 'mode', 'MainLocalAuthorityData');

program.parse();

const options = program.opts();

const outFile = options.outfile;
const mode = options.mode;

const outLines = [];

function getLaName(localAuthority) {
    return localAuthority["Local Authority Name"];
}

function getLaCustodianCode(localAuthority) {
    return localAuthority["Custodian Code"];
}

function getLaStatus(localAuthority) {
    switch (localAuthority["WHLG Status"]) {
        case "Live":
            return "Live";
        case "Not Taking Part": // legacy name
        case "NotTakingPart":
        case "LA has no funding":
            return "NoFunding";
        case "Not Participating":
        case "NotParticipating":
            return "NotParticipating"
        case "Taking Future Referrals":
        case "TakingFutureReferrals":
            return "TakingFutureReferrals";
        case "Pending":
            return "Pending";
        case "No Longer Participating":
        case "NoLongerParticipating":
            return "NoLongerParticipating";
        default:
            throw new Error(`LA in invalid state to determine status, ${JSON.stringify(localAuthority)}`);
    }
}

function getLaConsortiumName(localAuthority) {
    switch (localAuthority["Consortium/LA"]) {
        case "Consortium":
            return localAuthority["Consortium Name"];
        case "LA":
            return undefined;
        case "":
            return undefined;
        default:
            throw new Error(`LA in invalid state to determine consortium name, ${JSON.stringify(localAuthority)}`);
    }
}

function getLaWebsite(localAuthority) {
    return localAuthority["URL"];
}

function getLaConsortium(localAuthority, consortia) {
    return consortia.find(consortium => getConsortiumName(consortium) === getLaConsortiumName(localAuthority))
}

function getConsortiumCode(consortium) {
    return consortium["Consortium Code"];
}

function getConsortiumName(consortium) {
    return consortium["Consortium Name"];
}

function quoteConsortiumNameOrNull(consortiumName) {
    if (consortiumName === undefined) return "null"
    if (consortiumName === "") return "null"
    if (consortiumName === "None") return "null"

    return `"${consortiumName}"`
}

(async () => {
    const localAuthoritiesFile = readFileSync("local-authorities.csv","utf-8");
    const consortiaFile = readFileSync("consortia.csv","utf-8");

    const localAuthorities = (await csv.parse(localAuthoritiesFile, {columns: true, bom: true}).toArray()).sort((a, b) => {
        return getLaName(a).localeCompare(getLaName(b));
    });
    const consortia = (await csv.parse(consortiaFile, {columns: true, bom: true}).toArray()).sort((a, b) => {
        return getConsortiumCode(a).localeCompare(getConsortiumCode(b));
    });

    // overide for the one mode that needs to iterate consortia
    if (mode === "PortalConsortia") {
        for (const consortium of consortia) {
            const code = getConsortiumCode(consortium);
            const name = getConsortiumName(consortium);

            outLines.push(
                `        { "${code}", "${name}" },`
            );
        }

        writeFileSync(outFile, outLines.join("\n"));

        console.log(`finished! see ${outFile}`);
        return;
    }

    // all other modes iterate local authorities
    for (const localAuthority of localAuthorities) {
        const name = getLaName(localAuthority);
        const custodianCode = getLaCustodianCode(localAuthority);
        const status = getLaStatus(localAuthority);
        const consortiumName = getLaConsortiumName(localAuthority);
        const quotedConsortiumNameOrNull = quoteConsortiumNameOrNull(consortiumName);
        const website = getLaWebsite(localAuthority);
        const consortium = getLaConsortium(localAuthority, consortia);

        switch (mode) {
            case "MainLocalAuthorityData":
                outLines.push(
                    `        { "${custodianCode}", new LocalAuthorityDetails("${name}", LocalAuthorityStatus.${status}, "${website}", IncomeBandOptions[IncomeThreshold._36000], ${quotedConsortiumNameOrNull}) },`
                )
                break;
            case "MainLocalAuthorityConsortiums":
                outLines.push(
                    `            { "${custodianCode}", ${quotedConsortiumNameOrNull} },`
                )
                break;
            case "MainLocalAuthorityIncomeBands":
                outLines.push(
                    `            { "${custodianCode}", ThresholdAt36000 },`
                )
                break;
            case "MainLocalAuthorityNames":
                outLines.push(
                    `            { "${custodianCode}", "${name}" },`
                )
                break;
            case "MainLocalAuthorityStatuses":
                outLines.push(
                    `            { "${custodianCode}", ${status} },`
                )
                break;
            case "MainLocalAuthorityWebsiteUrls":
                outLines.push(
                    `            { "${custodianCode}", "${website}" },`
                )
                break;
            case "PortalLocalAuthorityNames":
                outLines.push(
                    `        { "${custodianCode}", "${name}" },`
                )
                break;
            case "PortalLocalAuthorityConsortiumCodes":
                if (consortium !== undefined) {
                    const consortiumCode = getConsortiumCode(consortium);
                    outLines.push(
                        `        { "${custodianCode}", "${consortiumCode}" },`
                    );
                }
                break;
            default:
                console.log("unknown mode");
                return;
        }
    }

    writeFileSync(outFile, outLines.join("\n"));

    console.log(`finished! see ${outFile}`);
})();
