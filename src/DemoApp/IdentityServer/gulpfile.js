var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var sass = require('gulp-sass');
var minifyCSS = require('gulp-clean-css');
var del = require('del');
var ts = require("gulp-typescript");
var tsProject = ts.createProject("tsconfig.json");

var distFolder = './wwwroot/dist/';
var jsFolder = `${distFolder}js/`;
var cssFolder = `${distFolder}css/`;

function processClean() {
    return del(`${distFolder}**`, { force: true });
}

function processTypescript() {
    return tsProject.src()
        .pipe(tsProject())
        .js.pipe(gulp.dest("dist"));
}

function processScripts() {
    return gulp
        .src([
            './node_modules/jquery/dist/jquery.js',
        ])
        .pipe(concat('bundle.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest(jsFolder));
}

function processFonts() {
    return gulp
        .src([
            './node_modules/font-awesome/fonts/**',
        ])
        .pipe(gulp.dest(`${distFolder}fonts/`));
}

function processSass() {
    return gulp
        .src([
            'Styles/web.scss'
        ])
        .pipe(sass())
        .on('error', sass.logError)
        .pipe(gulp.dest(cssFolder));
}

function processSassMin() {
    return gulp
        .src([
            'Styles/web.scss'
        ])
        .pipe(sass())
        .on('error', sass.logError)
        .pipe(minifyCSS())
        .pipe(concat('web.min.css'))
        .pipe(gulp.dest(cssFolder));
}

function processStyles() {
    return gulp
        .src([
            './node_modules/bootstrap/dist/css/bootstrap.css',
        ])
        .pipe(minifyCSS())
        .pipe(concat('bundle.min.css'))
        .pipe(gulp.dest(cssFolder));
}

var buildStyles = gulp.series(processStyles, processSass, processSassMin);
var build = gulp.parallel(buildStyles, processScripts);

gulp.task('clean', processClean);
gulp.task('styles', buildStyles);
gulp.task('sass', processSass);
gulp.task('sass:min', processSassMin);
gulp.task('fonts', processFonts);
gulp.task('scripts', processScripts);
gulp.task("typescript", processTypescript);
gulp.task('build', build);
