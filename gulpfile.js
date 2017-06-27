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

function target(framework, depends, config)
{
    gulp.task('build-' + framework, [depends], function() {
        return gulp
            .src('src/*.sln')
            .pipe(msbuild({
                configuration: config,
                toolsVersion: 14.0,
                targets: ['Clean', 'Build'],
                errorOnFail: true,
                stdout: true
            }));
    });

    gulp.task('test-' + framework, ['build-' + framework], function() {
        return gulp
            .src(['**/bin/**/*Tests.dll'], { read: false })
            .pipe(nunit({
                executable: 'nunit3-console.exe',
                teamcity: true,
                options: {
                    framework: 'net-4.5',
                    result: 'TestResults.xml'
                }
            }));
    });

    gulp.task('nuget-files-' + framework, ['test-' + framework], function() {
        return gulp.src('src/WebApi.StructureMap/bin/Release/WebApi.StructureMap.*')
            .pipe(gulp.dest('package/lib/' + framework));
    }); 

    gulp.task(framework, ['nuget-files-' + framework]); 
}

target('net452', 'assemblyInfo', 'Release-4.5.2');
target('net462', 'net452', 'Release');

gulp.task('nuget-package', ['net462'], function() {
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