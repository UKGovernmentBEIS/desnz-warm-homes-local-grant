var fs = require('fs');
var path = require('path');
var crypto = require('crypto');

var sass = require('sass');
var UglifyJS = require("uglify-js");


var pathToCurrentDirectory = './';
var pathToVisualStudioDebugDirectory = './bin/Debug/net8.0/';

var inputDirectory = './wwwroot';
var inputJsDirectory = './wwwroot/js';
var inputJsBundleDirectory = './wwwroot/js/bundle';
var outputDirectory = './wwwroot/compiled';


function makeOutputDirectoryIfItDoesNotExist(options) {
    function action(directory) {
        console.log(`Making output directory (if it does not exist) [${directory + outputDirectory}]`);

        if (!fs.existsSync(directory + outputDirectory)) {
            fs.mkdirSync(directory + outputDirectory);
        }
    }

    action(pathToCurrentDirectory);
    if (options.runningLocally) {
        action(pathToVisualStudioDebugDirectory);
    }
}

function deleteExistingCompiledCssAndJsFiles(options) {
    function action(directory) {
        console.log(`Deleting existing compiled CSS and JS files from output directory [${directory + outputDirectory}]`);
        var files = fs.readdirSync(directory + outputDirectory);

        files.forEach(function (fileName) {
            if (/app-.*.css/.test(fileName) || /app-.*.js/.test(fileName)) {
                var filePath = path.join(directory + outputDirectory, fileName);
                console.log(`Deleting file [${filePath}]`);
                fs.unlinkSync(filePath);
            }
        });
    }

    action(pathToCurrentDirectory);
    if (options.runningLocally) {
        action(pathToVisualStudioDebugDirectory);
    }
}

function compileSass(inputFile, outputFileNamePrefix, options) {
    function saveAction(directory) {
        // Generate the filename, based on the hash
        var outputFilePath = `${directory + outputDirectory}/${outputFileNamePrefix}-${hashResult}.css`;
        console.log(`Saving SASS to file [${outputFilePath}]`);

        // Save the SASS
        fs.writeFileSync(outputFilePath, renderResult.css);
    }

    console.log(`Compiling SASS from file [${inputFile}]`);

    // The GovUk frontend SASS currently causes build warnings when built with
    // Dart SASS. There are plans to eventually fix this, but they can't be
    // enacted right now.
    // So our only other option is to use a load path so that we can also use
    // the Quiet Deps option.
    var renderResult = sass.compile(inputFile, {
        loadPaths: [ 'node_modules/govuk-frontend' ],
        quietDeps: true
    });

    if (renderResult) {
        // Compute the hash of the compiled SASS
        var hash = crypto.createHash('sha256');
        hash.update(renderResult.css);
        var hashResult = hash.digest('hex');

        saveAction(pathToCurrentDirectory);
        if (options.runningLocally) {
            saveAction(pathToVisualStudioDebugDirectory);
        }
    }

}

function compileJs(options) {
    function saveAction(directory, hashResult, prefix) {
        // Generate the filename, based on the hash
        var outputFilePath = `${directory + outputDirectory}/${prefix ?? 'app'}-${hashResult}.js`;
        console.log(`Saving JS to file [${outputFilePath}]`);

        // Save the JS
        fs.writeFileSync(outputFilePath, minifyResult.code);
    }
    
    function saveToUniqueFile(code, prefix) {
        // Compute the hash of the compiled JS
        var hash = crypto.createHash('sha256');
        hash.update(code);
        var hashResult = hash.digest('hex');

        saveAction(pathToCurrentDirectory, hashResult, prefix);
        if (options.runningLocally) {
            saveAction(pathToVisualStudioDebugDirectory, hashResult, prefix);
        }
    }

    console.log(`Compiling JS bundle`);
    var bundleFiles = fs.readdirSync(inputJsBundleDirectory);

    var code = {};

    bundleFiles.forEach(function (fileName) {
        if (fileName.endsWith('.js')) {
            var filePath = path.join(inputJsBundleDirectory, fileName);
            var fileContents = fs.readFileSync(filePath, { encoding: 'utf8' });
            code[fileName] = fileContents;
        }
    });

    var minifyOptions = {
        keep_fnames: false,
        mangle: false     
    };
    var minifyResult = UglifyJS.minify(code, minifyOptions);

    if (minifyResult.code) {
        saveToUniqueFile(minifyResult.code)
    } else {
        console.log("MINIFY ERROR: " + minifyResult.error)
    }

    console.log(`Compiling individual JS files`);
    var files = fs.readdirSync(inputJsDirectory);

    files.forEach(function (fileName) {
        if (fileName.endsWith('.js')) {
            var filePath = path.join(inputJsDirectory, fileName);
            var fileContents = fs.readFileSync(filePath, { encoding: 'utf8' });
            saveToUniqueFile(fileContents, fileName.slice(0, -3));
        }
    });
}

async function fullRecompile(options) {
    makeOutputDirectoryIfItDoesNotExist(options);
    deleteExistingCompiledCssAndJsFiles(options);

    compileSass('./wwwroot/css/app.scss', 'app', options);
    compileSass('./wwwroot/css/app-ie8.scss', 'app-ie8', options);

    compileJs(options);
}

function stopOnCtrlC() {
    if (process.platform === "win32") {
        var rl = require("readline").createInterface({
            input: process.stdin,
            output: process.stdout
        });

        rl.on("SIGINT",
            function () {
                process.emit("SIGINT");
            });
    }

    process.on("SIGINT",
        function () {
            //graceful shutdown
            process.exit();
        });
}

var needsRecompile = false;

async function recompileWhenNeeded() {
    var snooze = ms => new Promise(resolve => setTimeout(resolve, ms));

    while (true) {
        await snooze(200);

        if (needsRecompile) {
            needsRecompile = false;
            fullRecompile({ runningLocally: true });
        }
    }
}

function watchAndAskForRecompile() {
    function fileChanged(eventType, filename) {
        if (filename && filename.indexOf('compiled') === -1) {
            needsRecompile = true;
        }
    }

    fs.watch(
        inputDirectory,
        {
            recursive: true
        },
        fileChanged
    );
}


if (process.argv.includes('--watch')) {
    fullRecompile({ runningLocally: true });

    console.log('');
    console.log('Watching for changes');
    console.log('Press Ctrl+C to exit');
    console.log('');

    stopOnCtrlC();
    recompileWhenNeeded();
    watchAndAskForRecompile();
}
else
{
    fullRecompile({});
}
