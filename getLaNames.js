// Usage: `node .\getLaNames.js`
// This script takes the contents of LocalAuthorityDetailsByCustodianCode.cs and updates the local authority name and
// website from https://local-links-manager.publishing.service.gov.uk/api
// It likely will only be used once, but could potentially be useful in the future. If so, be wary of overwriting any
// HUG2 specific web addresses that may have been added. Also carefully check any problem entries logged at the end of
// the script

function sleep(ms) {
    return new Promise((resolve) => {
        setTimeout(resolve, ms);
    });
}

const localAuthorityDataCode =
    "        { \"9052\", new LocalAuthorityDetails(\"Aberdeenshire\", Hug2Status.NotTakingPart, \"https://www.aberdeenshire.gov.uk/\") },\n" +
    "        { \"3805\", new LocalAuthorityDetails(\"Adur\", Hug2Status.Pending, \"\") },\n" +
    "        { \"905\", new LocalAuthorityDetails(\"Allerdale\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1005\", new LocalAuthorityDetails(\"Amber Valley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9053\", new LocalAuthorityDetails(\"Angus\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9054\", new LocalAuthorityDetails(\"Argyll and Bute\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3810\", new LocalAuthorityDetails(\"Arun\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3005\", new LocalAuthorityDetails(\"Ashfield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2205\", new LocalAuthorityDetails(\"Ashford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3505\", new LocalAuthorityDetails(\"Babergh\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5060\", new LocalAuthorityDetails(\"Barking and Dagenham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5090\", new LocalAuthorityDetails(\"Barnet\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4405\", new LocalAuthorityDetails(\"Barnsley\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"910\", new LocalAuthorityDetails(\"Barrow-In-Furness\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1505\", new LocalAuthorityDetails(\"Basildon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1705\", new LocalAuthorityDetails(\"Basingstoke and Deane\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3010\", new LocalAuthorityDetails(\"Bassetlaw\", Hug2Status.Pending, \"\") },\n" +
    "        { \"114\", new LocalAuthorityDetails(\"Bath and North East Somerset\", Hug2Status.Pending, \"\") },\n" +
    "        { \"235\", new LocalAuthorityDetails(\"Bedford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5120\", new LocalAuthorityDetails(\"Bexley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4605\", new LocalAuthorityDetails(\"Birmingham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2405\", new LocalAuthorityDetails(\"Blaby\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2372\", new LocalAuthorityDetails(\"Blackburn With Darwen\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2373\", new LocalAuthorityDetails(\"Blackpool\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6910\", new LocalAuthorityDetails(\"Blaenau Gwent\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1010\", new LocalAuthorityDetails(\"Bolsover\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4205\", new LocalAuthorityDetails(\"Bolton\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2505\", new LocalAuthorityDetails(\"Boston\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1260\", new LocalAuthorityDetails(\"Bournemouth Christchurch and Poole\", Hug2Status.Pending, \"\") },\n" +
    "        { \"335\", new LocalAuthorityDetails(\"Bracknell Forest\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4705\", new LocalAuthorityDetails(\"Bradford Mdc\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1510\", new LocalAuthorityDetails(\"Braintree\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2605\", new LocalAuthorityDetails(\"Breckland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5150\", new LocalAuthorityDetails(\"Brent\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1515\", new LocalAuthorityDetails(\"Brentwood\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6915\", new LocalAuthorityDetails(\"Bridgend County Borough\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1445\", new LocalAuthorityDetails(\"Brighton & Hove\", Hug2Status.Pending, \"\") },\n" +
    "        { \"116\", new LocalAuthorityDetails(\"Bristol\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2610\", new LocalAuthorityDetails(\"Broadland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1805\", new LocalAuthorityDetails(\"Bromsgrove\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1905\", new LocalAuthorityDetails(\"Broxbourne\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3015\", new LocalAuthorityDetails(\"Broxtowe\", Hug2Status.Pending, \"\") },\n" +
    "        { \"440\", new LocalAuthorityDetails(\"Buckinghamshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2315\", new LocalAuthorityDetails(\"Burnley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4210\", new LocalAuthorityDetails(\"Bury\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"6920\", new LocalAuthorityDetails(\"Caerphilly County Borough\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4710\", new LocalAuthorityDetails(\"Calderdale\", Hug2Status.Pending, \"\") },\n" +
    "        { \"505\", new LocalAuthorityDetails(\"Cambridge\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5210\", new LocalAuthorityDetails(\"Camden\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3405\", new LocalAuthorityDetails(\"Cannock Chase\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2210\", new LocalAuthorityDetails(\"Canterbury\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6815\", new LocalAuthorityDetails(\"Cardiff\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"915\", new LocalAuthorityDetails(\"Carlisle\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6825\", new LocalAuthorityDetails(\"Carmarthenshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1520\", new LocalAuthorityDetails(\"Castle Point\", Hug2Status.Pending, \"\") },\n" +
    "        { \"240\", new LocalAuthorityDetails(\"Central Bedfordshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"6820\", new LocalAuthorityDetails(\"Ceredigion\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2410\", new LocalAuthorityDetails(\"Charnwood\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1525\", new LocalAuthorityDetails(\"Chelmsford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1605\", new LocalAuthorityDetails(\"Cheltenham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3105\", new LocalAuthorityDetails(\"Cherwell\", Hug2Status.Pending, \"\") },\n" +
    "        { \"660\", new LocalAuthorityDetails(\"Cheshire East\", Hug2Status.Pending, \"\") },\n" +
    "        { \"665\", new LocalAuthorityDetails(\"Cheshire West and Chester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1015\", new LocalAuthorityDetails(\"Chesterfield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3815\", new LocalAuthorityDetails(\"Chichester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2320\", new LocalAuthorityDetails(\"Chorley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9051\", new LocalAuthorityDetails(\"City of Aberdeen\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9059\", new LocalAuthorityDetails(\"City of Dundee\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9064\", new LocalAuthorityDetails(\"City of Edinburgh\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9067\", new LocalAuthorityDetails(\"City of Glasgow\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"5030\", new LocalAuthorityDetails(\"City of London\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3455\", new LocalAuthorityDetails(\"City of Stoke-On-Trent\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"5990\", new LocalAuthorityDetails(\"City of Westminster\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9056\", new LocalAuthorityDetails(\"Clackmannan\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1530\", new LocalAuthorityDetails(\"Colchester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6905\", new LocalAuthorityDetails(\"Conwy\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"920\", new LocalAuthorityDetails(\"Copeland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"840\", new LocalAuthorityDetails(\"Cornwall\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1610\", new LocalAuthorityDetails(\"Cotswold\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4610\", new LocalAuthorityDetails(\"Coventry\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2705\", new LocalAuthorityDetails(\"Craven\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3820\", new LocalAuthorityDetails(\"Crawley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5240\", new LocalAuthorityDetails(\"Croydon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"940\", new LocalAuthorityDetails(\"Cumberland\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1910\", new LocalAuthorityDetails(\"Dacorum\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1350\", new LocalAuthorityDetails(\"Darlington\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2215\", new LocalAuthorityDetails(\"Dartford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6830\", new LocalAuthorityDetails(\"Denbighshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1055\", new LocalAuthorityDetails(\"Derby\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1045\", new LocalAuthorityDetails(\"Derbyshire Dales\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4410\", new LocalAuthorityDetails(\"Doncaster\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1265\", new LocalAuthorityDetails(\"Dorset\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2220\", new LocalAuthorityDetails(\"Dover\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4615\", new LocalAuthorityDetails(\"Dudley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9058\", new LocalAuthorityDetails(\"Dumfries and Galloway\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1355\", new LocalAuthorityDetails(\"Durham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5270\", new LocalAuthorityDetails(\"Ealing\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9060\", new LocalAuthorityDetails(\"East Ayrshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"510\", new LocalAuthorityDetails(\"East Cambridgeshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1105\", new LocalAuthorityDetails(\"East Devon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9061\", new LocalAuthorityDetails(\"East Dunbartonshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1710\", new LocalAuthorityDetails(\"East Hampshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1915\", new LocalAuthorityDetails(\"East Hertfordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2510\", new LocalAuthorityDetails(\"East Lindsey\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9062\", new LocalAuthorityDetails(\"East Lothian\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9063\", new LocalAuthorityDetails(\"East Renfrewshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2001\", new LocalAuthorityDetails(\"East Riding of Yorkshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3410\", new LocalAuthorityDetails(\"East Staffordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3540\", new LocalAuthorityDetails(\"East Suffolk\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1410\", new LocalAuthorityDetails(\"Eastbourne\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1715\", new LocalAuthorityDetails(\"Eastleigh\", Hug2Status.Pending, \"\") },\n" +
    "        { \"925\", new LocalAuthorityDetails(\"Eden\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3605\", new LocalAuthorityDetails(\"Elmbridge\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5300\", new LocalAuthorityDetails(\"Enfield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1535\", new LocalAuthorityDetails(\"Epping Forest\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3610\", new LocalAuthorityDetails(\"Epsom and Ewell\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1025\", new LocalAuthorityDetails(\"Erewash\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1110\", new LocalAuthorityDetails(\"Exeter\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9065\", new LocalAuthorityDetails(\"Falkirk\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1720\", new LocalAuthorityDetails(\"Fareham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"515\", new LocalAuthorityDetails(\"Fenland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9066\", new LocalAuthorityDetails(\"Fife\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"6835\", new LocalAuthorityDetails(\"Flintshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2250\", new LocalAuthorityDetails(\"Folkestone and Hythe\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1615\", new LocalAuthorityDetails(\"Forest of Dean\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2325\", new LocalAuthorityDetails(\"Fylde\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4505\", new LocalAuthorityDetails(\"Gateshead\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3020\", new LocalAuthorityDetails(\"Gedling\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1620\", new LocalAuthorityDetails(\"Gloucester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1725\", new LocalAuthorityDetails(\"Gosport\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2230\", new LocalAuthorityDetails(\"Gravesham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2615\", new LocalAuthorityDetails(\"Great Yarmouth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5330\", new LocalAuthorityDetails(\"Greenwich\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3615\", new LocalAuthorityDetails(\"Guildford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6810\", new LocalAuthorityDetails(\"Gwynedd\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"5360\", new LocalAuthorityDetails(\"Hackney\", Hug2Status.Pending, \"\") },\n" +
    "        { \"650\", new LocalAuthorityDetails(\"Halton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2710\", new LocalAuthorityDetails(\"Hambleton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5390\", new LocalAuthorityDetails(\"Hammersmith and Fulham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2415\", new LocalAuthorityDetails(\"Harborough\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1540\", new LocalAuthorityDetails(\"Harlow\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2715\", new LocalAuthorityDetails(\"Harrogate\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5450\", new LocalAuthorityDetails(\"Harrow\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1730\", new LocalAuthorityDetails(\"Hart\", Hug2Status.Pending, \"\") },\n" +
    "        { \"724\", new LocalAuthorityDetails(\"Hartlepool\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1415\", new LocalAuthorityDetails(\"Hastings\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1735\", new LocalAuthorityDetails(\"Havant\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5480\", new LocalAuthorityDetails(\"Havering\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1850\", new LocalAuthorityDetails(\"Herefordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1920\", new LocalAuthorityDetails(\"Hertsmere\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1030\", new LocalAuthorityDetails(\"High Peak\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9068\", new LocalAuthorityDetails(\"Highland\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"5510\", new LocalAuthorityDetails(\"Hillingdon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2420\", new LocalAuthorityDetails(\"Hinckley and Bosworth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3825\", new LocalAuthorityDetails(\"Horsham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"520\", new LocalAuthorityDetails(\"Huntingdonshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2330\", new LocalAuthorityDetails(\"Hyndburn\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9069\", new LocalAuthorityDetails(\"Inverclyde\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3515\", new LocalAuthorityDetails(\"Ipswich\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6805\", new LocalAuthorityDetails(\"Isle of Anglesey\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2114\", new LocalAuthorityDetails(\"Isle of Wight\", Hug2Status.Pending, \"\") },\n" +
    "        { \"835\", new LocalAuthorityDetails(\"Isles of Scilly\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5570\", new LocalAuthorityDetails(\"Islington\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5600\", new LocalAuthorityDetails(\"Kensington and Chelsea\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2635\", new LocalAuthorityDetails(\"Kings Lynn and West Norfolk\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2004\", new LocalAuthorityDetails(\"Kingston Upon Hull\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"5630\", new LocalAuthorityDetails(\"Kingston Upon Thames\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4715\", new LocalAuthorityDetails(\"Kirklees\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4305\", new LocalAuthorityDetails(\"Knowsley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5660\", new LocalAuthorityDetails(\"Lambeth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2335\", new LocalAuthorityDetails(\"Lancaster City\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4720\", new LocalAuthorityDetails(\"Leeds\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2465\", new LocalAuthorityDetails(\"Leicester City\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1425\", new LocalAuthorityDetails(\"Lewes\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5690\", new LocalAuthorityDetails(\"Lewisham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3415\", new LocalAuthorityDetails(\"Lichfield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2515\", new LocalAuthorityDetails(\"Lincoln\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4310\", new LocalAuthorityDetails(\"Liverpool\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5180\", new LocalAuthorityDetails(\"London Borough of Bromley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5420\", new LocalAuthorityDetails(\"London Borough of Haringey\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5540\", new LocalAuthorityDetails(\"London Borough of Hounslow\", Hug2Status.Pending, \"\") },\n" +
    "        { \"230\", new LocalAuthorityDetails(\"Luton\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2235\", new LocalAuthorityDetails(\"Maidstone\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1545\", new LocalAuthorityDetails(\"Maldon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1820\", new LocalAuthorityDetails(\"Malvern Hills\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4215\", new LocalAuthorityDetails(\"Manchester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3025\", new LocalAuthorityDetails(\"Mansfield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2280\", new LocalAuthorityDetails(\"Medway\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2430\", new LocalAuthorityDetails(\"Melton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3305\", new LocalAuthorityDetails(\"Mendip\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6925\", new LocalAuthorityDetails(\"Merthyr Tydfil Ua\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"5720\", new LocalAuthorityDetails(\"Merton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1135\", new LocalAuthorityDetails(\"Mid Devon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3520\", new LocalAuthorityDetails(\"Mid Suffolk\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3830\", new LocalAuthorityDetails(\"Mid Sussex\", Hug2Status.Pending, \"\") },\n" +
    "        { \"734\", new LocalAuthorityDetails(\"Middlesbrough\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9070\", new LocalAuthorityDetails(\"Midlothian\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"435\", new LocalAuthorityDetails(\"Milton Keynes\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3620\", new LocalAuthorityDetails(\"Mole Valley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6840\", new LocalAuthorityDetails(\"Monmouthshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9071\", new LocalAuthorityDetails(\"Moray\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"6930\", new LocalAuthorityDetails(\"Neath Port Talbot\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1740\", new LocalAuthorityDetails(\"New Forest\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3030\", new LocalAuthorityDetails(\"Newark and Sherwood\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4510\", new LocalAuthorityDetails(\"Newcastle Upon Tyne\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3420\", new LocalAuthorityDetails(\"Newcastle-Under-Lyme\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5750\", new LocalAuthorityDetails(\"Newham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6935\", new LocalAuthorityDetails(\"Newport\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9072\", new LocalAuthorityDetails(\"North Ayrshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1115\", new LocalAuthorityDetails(\"North Devon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1035\", new LocalAuthorityDetails(\"North East Derbyshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2002\", new LocalAuthorityDetails(\"North East Lincolnshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1925\", new LocalAuthorityDetails(\"North Hertfordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2520\", new LocalAuthorityDetails(\"North Kesteven\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9073\", new LocalAuthorityDetails(\"North Lanarkshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2003\", new LocalAuthorityDetails(\"North Lincolnshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2620\", new LocalAuthorityDetails(\"North Norfolk\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2840\", new LocalAuthorityDetails(\"North Northamptonshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"121\", new LocalAuthorityDetails(\"North Somerset\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4515\", new LocalAuthorityDetails(\"North Tyneside\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3705\", new LocalAuthorityDetails(\"North Warwickshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2435\", new LocalAuthorityDetails(\"North West Leicestershire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2745\", new LocalAuthorityDetails(\"North Yorkshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2935\", new LocalAuthorityDetails(\"Northumberland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2625\", new LocalAuthorityDetails(\"Norwich\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3060\", new LocalAuthorityDetails(\"Nottingham City\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3710\", new LocalAuthorityDetails(\"Nuneaton and Bedworth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2440\", new LocalAuthorityDetails(\"Oadby and Wigston\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4220\", new LocalAuthorityDetails(\"Oldham\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"7655\", new LocalAuthorityDetails(\"Ordnance Survey\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"9000\", new LocalAuthorityDetails(\"Orkney Islands\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3110\", new LocalAuthorityDetails(\"Oxford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6845\", new LocalAuthorityDetails(\"Pembrokeshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2340\", new LocalAuthorityDetails(\"Pendle\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9074\", new LocalAuthorityDetails(\"Perth and Kinross\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"540\", new LocalAuthorityDetails(\"Peterborough\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1160\", new LocalAuthorityDetails(\"Plymouth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1775\", new LocalAuthorityDetails(\"Portsmouth City Council\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6850\", new LocalAuthorityDetails(\"Powys\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2345\", new LocalAuthorityDetails(\"Preston\", Hug2Status.Pending, \"\") },\n" +
    "        { \"345\", new LocalAuthorityDetails(\"Reading\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5780\", new LocalAuthorityDetails(\"Redbridge\", Hug2Status.Pending, \"\") },\n" +
    "        { \"728\", new LocalAuthorityDetails(\"Redcar and Cleveland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1825\", new LocalAuthorityDetails(\"Redditch\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3625\", new LocalAuthorityDetails(\"Reigate and Banstead\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9075\", new LocalAuthorityDetails(\"Renfrewshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"6940\", new LocalAuthorityDetails(\"Rhondda Cynon Taff\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2350\", new LocalAuthorityDetails(\"Ribble Valley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5810\", new LocalAuthorityDetails(\"Richmond Upon Thames\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2720\", new LocalAuthorityDetails(\"Richmondshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4225\", new LocalAuthorityDetails(\"Rochdale\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1550\", new LocalAuthorityDetails(\"Rochford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2355\", new LocalAuthorityDetails(\"Rossendale\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1430\", new LocalAuthorityDetails(\"Rother\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4415\", new LocalAuthorityDetails(\"Rotherham\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3715\", new LocalAuthorityDetails(\"Rugby\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3630\", new LocalAuthorityDetails(\"Runnymede\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3040\", new LocalAuthorityDetails(\"Rushcliffe\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1750\", new LocalAuthorityDetails(\"Rushmoor\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2470\", new LocalAuthorityDetails(\"Rutland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2725\", new LocalAuthorityDetails(\"Ryedale\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4230\", new LocalAuthorityDetails(\"Salford\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4620\", new LocalAuthorityDetails(\"Sandwell\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2730\", new LocalAuthorityDetails(\"Scarborough\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9055\", new LocalAuthorityDetails(\"Scottish Borders\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3310\", new LocalAuthorityDetails(\"Sedgemoor\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4320\", new LocalAuthorityDetails(\"Sefton Council\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2735\", new LocalAuthorityDetails(\"Selby\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2245\", new LocalAuthorityDetails(\"Sevenoaks\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4420\", new LocalAuthorityDetails(\"Sheffield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9010\", new LocalAuthorityDetails(\"Shetland Islands\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3245\", new LocalAuthorityDetails(\"Shropshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"350\", new LocalAuthorityDetails(\"Slough\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4625\", new LocalAuthorityDetails(\"Solihull\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3300\", new LocalAuthorityDetails(\"Somerset\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3330\", new LocalAuthorityDetails(\"Somerset West and Taunton \", Hug2Status.Pending, \"\") },\n" +
    "        { \"9076\", new LocalAuthorityDetails(\"South Ayrshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"530\", new LocalAuthorityDetails(\"South Cambridgeshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1040\", new LocalAuthorityDetails(\"South Derbyshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"119\", new LocalAuthorityDetails(\"South Gloucestershire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1125\", new LocalAuthorityDetails(\"South Hams\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2525\", new LocalAuthorityDetails(\"South Holland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2530\", new LocalAuthorityDetails(\"South Kesteven\", Hug2Status.Pending, \"\") },\n" +
    "        { \"930\", new LocalAuthorityDetails(\"South Lakeland\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9077\", new LocalAuthorityDetails(\"South Lanarkshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2630\", new LocalAuthorityDetails(\"South Norfolk\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3115\", new LocalAuthorityDetails(\"South Oxfordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2360\", new LocalAuthorityDetails(\"South Ribble\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3325\", new LocalAuthorityDetails(\"South Somerset\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3430\", new LocalAuthorityDetails(\"South Staffordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4520\", new LocalAuthorityDetails(\"South Tyneside\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1780\", new LocalAuthorityDetails(\"Southampton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1590\", new LocalAuthorityDetails(\"Southend-On-Sea\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5840\", new LocalAuthorityDetails(\"Southwark\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3635\", new LocalAuthorityDetails(\"Spelthorne\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1930\", new LocalAuthorityDetails(\"St Albans\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4315\", new LocalAuthorityDetails(\"St Helens Council\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3425\", new LocalAuthorityDetails(\"Stafford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3435\", new LocalAuthorityDetails(\"Staffordshire Moorlands\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1935\", new LocalAuthorityDetails(\"Stevenage\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9078\", new LocalAuthorityDetails(\"Stirling\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4235\", new LocalAuthorityDetails(\"Stockport\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"738\", new LocalAuthorityDetails(\"Stockton-On-Tees\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3720\", new LocalAuthorityDetails(\"Stratford-On-Avon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1625\", new LocalAuthorityDetails(\"Stroud\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4525\", new LocalAuthorityDetails(\"Sunderland\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3640\", new LocalAuthorityDetails(\"Surrey Heath\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5870\", new LocalAuthorityDetails(\"Sutton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2255\", new LocalAuthorityDetails(\"Swale\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6855\", new LocalAuthorityDetails(\"Swansea\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3935\", new LocalAuthorityDetails(\"Swindon\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4240\", new LocalAuthorityDetails(\"Tameside\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3445\", new LocalAuthorityDetails(\"Tamworth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3645\", new LocalAuthorityDetails(\"Tandridge\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1130\", new LocalAuthorityDetails(\"Teignbridge\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3240\", new LocalAuthorityDetails(\"Telford and Wrekin\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1560\", new LocalAuthorityDetails(\"Tendring\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1760\", new LocalAuthorityDetails(\"Test Valley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1630\", new LocalAuthorityDetails(\"Tewkesbury\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2260\", new LocalAuthorityDetails(\"Thanet District\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1940\", new LocalAuthorityDetails(\"Three Rivers\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1595\", new LocalAuthorityDetails(\"Thurrock\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2265\", new LocalAuthorityDetails(\"Tonbridge and Malling\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1165\", new LocalAuthorityDetails(\"Torbay\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6945\", new LocalAuthorityDetails(\"Torfaen\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1145\", new LocalAuthorityDetails(\"Torridge\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5900\", new LocalAuthorityDetails(\"Tower Hamlets\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4245\", new LocalAuthorityDetails(\"Trafford\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2270\", new LocalAuthorityDetails(\"Tunbridge Wells\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1570\", new LocalAuthorityDetails(\"Uttlesford\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6950\", new LocalAuthorityDetails(\"Vale of Glamorgan\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3120\", new LocalAuthorityDetails(\"Vale of White Horse\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4725\", new LocalAuthorityDetails(\"Wakefield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4630\", new LocalAuthorityDetails(\"Walsall\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5930\", new LocalAuthorityDetails(\"Waltham Forest\", Hug2Status.Pending, \"\") },\n" +
    "        { \"5960\", new LocalAuthorityDetails(\"Wandsworth\", Hug2Status.Pending, \"\") },\n" +
    "        { \"655\", new LocalAuthorityDetails(\"Warrington\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3725\", new LocalAuthorityDetails(\"Warwick\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1945\", new LocalAuthorityDetails(\"Watford\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3650\", new LocalAuthorityDetails(\"Waverley\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1435\", new LocalAuthorityDetails(\"Wealden\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1950\", new LocalAuthorityDetails(\"Welwyn Hatfield\", Hug2Status.Pending, \"\") },\n" +
    "        { \"340\", new LocalAuthorityDetails(\"West Berkshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1150\", new LocalAuthorityDetails(\"West Devon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9057\", new LocalAuthorityDetails(\"West Dunbartonshire\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2365\", new LocalAuthorityDetails(\"West Lancashire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2535\", new LocalAuthorityDetails(\"West Lindsey\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9079\", new LocalAuthorityDetails(\"West Lothian\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"2845\", new LocalAuthorityDetails(\"West Northamptonshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3125\", new LocalAuthorityDetails(\"West Oxfordshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3545\", new LocalAuthorityDetails(\"West Suffolk\", Hug2Status.Pending, \"\") },\n" +
    "        { \"9020\", new LocalAuthorityDetails(\"Western Isles\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"935\", new LocalAuthorityDetails(\"Westmorland and Furness\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"4250\", new LocalAuthorityDetails(\"Wigan\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"3940\", new LocalAuthorityDetails(\"Wiltshire\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1765\", new LocalAuthorityDetails(\"Winchester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"355\", new LocalAuthorityDetails(\"Windsor and Maidenhead\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4325\", new LocalAuthorityDetails(\"Wirral\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3655\", new LocalAuthorityDetails(\"Woking\", Hug2Status.Pending, \"\") },\n" +
    "        { \"360\", new LocalAuthorityDetails(\"Wokingham\", Hug2Status.Pending, \"\") },\n" +
    "        { \"4635\", new LocalAuthorityDetails(\"Wolverhampton\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1835\", new LocalAuthorityDetails(\"Worcester\", Hug2Status.Pending, \"\") },\n" +
    "        { \"3835\", new LocalAuthorityDetails(\"Worthing\", Hug2Status.Pending, \"\") },\n" +
    "        { \"6955\", new LocalAuthorityDetails(\"Wrexham\", Hug2Status.NotTakingPart, \"\") },\n" +
    "        { \"1840\", new LocalAuthorityDetails(\"Wychavon\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2370\", new LocalAuthorityDetails(\"Wyre\", Hug2Status.Pending, \"\") },\n" +
    "        { \"1845\", new LocalAuthorityDetails(\"Wyre Forest\", Hug2Status.Pending, \"\") },\n" +
    "        { \"2741\", new LocalAuthorityDetails(\"City of York Council\", Hug2Status.Pending, \"\") },";

const codeLineRegex = /(\s+\{ ")(\d+)(", new LocalAuthorityDetails\(").*(", Hug2Status.*, ").*("\) },)/;

(async () => {
    var results = [];
    var resultTracker = {};
    var problemCodes = {};

    var codeLines = localAuthorityDataCode.split("\n");
    
    for (var j=0; j < codeLines.length; j++) {
        var codeLine = codeLines[j]
        var regexResult = codeLineRegex.exec(codeLine);
        
        if (regexResult) {
            var laCode = regexResult[2];
            const res = await fetch('https://local-links-manager.publishing.service.gov.uk/api/local-authority?local_custodian_code=' + laCode);
            if (res.ok) {
                const data = await res.json();

                var laDetails = null;

                for (var i = 0; i < data.local_authorities.length; i++) {
                    tier = data.local_authorities[i].tier;
                    if (tier === "district" || tier === "unitary") {
                        if (resultTracker[laCode]) {
                            problemCodes[laCode] = "Multiple results found for code " + laCode;
                        }
                        var newCodeLine = codeLine.replace(codeLineRegex, "$1$2$3" + data.local_authorities[i].name + "$4" + data.local_authorities[i].homepage_url + "$5");
                        // Log as we go so we don't have to re-request entries if there are errors part way.
                        console.log(newCodeLine);
                        results.push(newCodeLine);
                        resultTracker[laCode] = true;
                    }
                }
                
                if (!resultTracker[laCode]) {
                    problemCodes[laCode] = "No results found for code " + laCode;
                }
            }
            else {
                problemCodes[laCode] = "Bad result from API: " + await res.text()
            }
                
        }
        else {
            throw "Couldn't parse code line: " + codeLine;
        }
        
        // Don't flood the API
        await sleep(1000);
    }
    
    console.log("PROBLEMS:");
    console.log(problemCodes);
})()