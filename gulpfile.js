var gulp = require('gulp'),
    args = require('yargs').argv,
    assemblyInfo = require('gulp-dotnet-assembly-info'),
    msbuild = require('gulp-msbuild'),
    nunit = require('gulp-nunit-runner'),
    Nuget = require('nuget-runner');

gulp.task('deploy', ['nuget-push']);

gulp.task('ci', ['nuget-package']);

gulp.task('assemblyInfo', function() {
    return gulp
        .src('**/AssemblyInfo.cs')
        .pipe(assemblyInfo({
            version: args.buildVersion,
            fileVersion: args.buildVersion,
            copyright: function(value) { 
                return 'Copyright © ' + new Date().getFullYear() + ' Setec Astronomy.';
            }
        }))
        .pipe(gulp.dest('.'));
});

var build = function(config) {
    return gulp
        .src('src/*.sln')
        .pipe(msbuild({
            configuration: config,
            toolsVersion: 14.0,
            targets: ['Clean', 'Build'],
            errorOnFail: true,
            stdout: true
        }));
};

var test = function() {
    return gulp
        .src(['**/bin/**/*Tests.dll'], { read: false })
        .pipe(nunit({
            executable: 'nunit3-console.exe',
            teamcity: true,
            options: {
                framework: 'net-4.5'
            }
        }));
};

var copyFiles = function(folder) {
    return gulp.src('src/WebApi.StructureMap/bin/Release/WebApi.StructureMap.*')
        .pipe(gulp.dest('package/lib/' + folder));
};

gulp.task('build-net452', ['assemblyInfo'], function() {
    return build('Release-4.5.2');
});

gulp.task('test-net452', ['build-net452'], test);

gulp.task('nuget-files-net452', ['test-net452'], function() {
    return copyFiles('net452');
});

gulp.task('build-net462', ['nuget-files-net452'], function() {
    return build('Release');
});

gulp.task('test-net462', ['build-net462'], test);

gulp.task('nuget-files-net462', ['test-net462'], function() {
    return copyFiles('net462');
});

gulp.task('nuget-package', ['nuget-files-net462'], function() {
    return Nuget()
        .pack({
            spec: 'WebApi.StructureMap.nuspec',
            basePath: 'package',
            version: args.buildVersion
        });
});

gulp.task('nuget-push', ['nuget-package'], function() {
    return Nuget().push('*.nupkg', { 
        apiKey: args.nugetApiKey, 
        source: ['https://www.nuget.org/api/v2/package'] 
    });
});