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

gulp.task('build', ['assemblyInfo'], function() {
    return gulp
        .src('src/*.sln')
        .pipe(msbuild({
            toolsVersion: 14.0,
            targets: ['Clean', 'Build'],
            errorOnFail: true,
            stdout: true
        }));
});

gulp.task('test', ['build'], function () {
    return gulp
        .src(['**/bin/**/*Tests.dll'], { read: false })
        .pipe(nunit({
            executable: 'nunit3-console.exe',
            teamcity: true,
            options: {
                framework: 'net-4.5'
            }
        }));
});

gulp.task('nuget-package', ['test'], function() {

    gulp.src('src/WebApi.StructureMap/bin/Release/WebApi.StructureMap.*')
        .pipe(gulp.dest('package/lib'));

    return Nuget()
        .pack({
            spec: 'WebApi.StructureMap.nuspec',
            basePath: 'package',
            version: args.buildVersion
        });
});

gulp.task('nuget-push', ['nuget-package'], function() {
    return Nuget({ apiKey: args.nugetApiKey }).push('*.nupkg');
});