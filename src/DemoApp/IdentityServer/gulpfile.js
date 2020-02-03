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

////////////////////////////////////////////////////////////////////////////////////////////////////
var identityServerAdminDistFolder = './wwwroot/id4config/dist/';
var identityServerAdminJsFolder = `${identityServerAdminDistFolder}js/`;
var identityServerAdminCssFolder = `${identityServerAdminDistFolder}css/`;

function processIdentityServerAdminClean() {
    return del(`${identityServerAdminDistFolder}**`, { force: true });
}

function processIdentityServerAdminScripts() {
    return gulp
        .src([
            './node_modules/jquery/dist/jquery.js',
            './node_modules/jquery-validation/dist/jquery.validate.js',
            './node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js',
            './node_modules/popper.js/dist/umd/popper.js',
            './node_modules/bootstrap/dist/js/bootstrap.js',
            './node_modules/holderjs/holder.js',
            './node_modules/knockout/build/output/knockout-latest.js',
            './node_modules/toastr/toastr.js',
            './node_modules/moment/min/moment.min.js',
            './node_modules/tempusdominus-bootstrap-4/build/js/tempusdominus-bootstrap-4.js',
            './node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js',
            './node_modules/bootstrap-datepicker/dist/locales/bootstrap-datepicker.fa.min.js',
            './node_modules/bootstrap-datepicker/dist/locales/bootstrap-datepicker.fr.min.js',
            './node_modules/bootstrap-datepicker/dist/locales/bootstrap-datepicker.ru.min.js',
            './node_modules/bootstrap-datepicker/dist/locales/bootstrap-datepicker.sv.min.js',
            './node_modules/bootstrap-datepicker/dist/locales/bootstrap-datepicker.zh-CN.min.js',
            './IdentityServerAdmin/Scripts/App/components/Menu.js',
            './IdentityServerAdmin/Scripts/App/components/Picker.es5.js',
            './IdentityServerAdmin/Scripts/App/components/Theme.js',
            './IdentityServerAdmin/Scripts/App/helpers/FormMvcHelpers.js',
            './IdentityServerAdmin/Scripts/App/helpers/jsontree.min.js',
            './IdentityServerAdmin/Scripts/App/helpers/DateTimeHelpers.js',
            './IdentityServerAdmin/Scripts/App/pages/ErrorsLog.js',
            './IdentityServerAdmin/Scripts/App/pages/AuditLog.js',
            './IdentityServerAdmin/Scripts/App/pages/Secrets.js',
            './IdentityServerAdmin/Scripts/App/components/DatePicker.js'
        ])
        .pipe(concat('bundle.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest(identityServerAdminJsFolder));
}

function processIdentityServerAdminFonts() {
    return gulp
        .src(['./node_modules/font-awesome/fonts/**', './node_modules/open-iconic/font/fonts/**'])
        .pipe(gulp.dest(`${identityServerAdminDistFolder}fonts/`));
}

function processIdentityServerAdminSass() {
    return gulp
        .src('IdentityServerAdminStyles/web.scss')
        .pipe(sass())
        .on('error', sass.logError)
        .pipe(gulp.dest(identityServerAdminCssFolder));
}

function processIdentityServerAdminSassMin() {
    return gulp
        .src('IdentityServerAdminStyles/web.scss')
        .pipe(sass())
        .on('error', sass.logError)
        .pipe(minifyCSS())
        .pipe(concat('web.min.css'))
        .pipe(gulp.dest(identityServerAdminCssFolder));
}

function processIdentityServerAdminStyles() {
    return gulp
        .src([
            './node_modules/bootstrap/dist/css/bootstrap.css',
            './node_modules/toastr/build/toastr.css',
            './node_modules/open-iconic/font/css/open-iconic-bootstrap.css',
            './node_modules/font-awesome/css/font-awesome.css',
            './node_modules/tempusdominus-bootstrap-4/build/css/tempusdominus-bootstrap-4.css',
            './node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css',
            './IdentityServerAdmin/Styles/controls/jsontree.css'
        ])
        .pipe(minifyCSS())
        .pipe(concat('bundle.min.css'))
        .pipe(gulp.dest(identityServerAdminCssFolder));
}

var buildIdnetityServerAdminStyles = gulp.series(processIdentityServerAdminStyles, processIdentityServerAdminSass, processIdentityServerAdminSassMin);
var buildIdentityServerAdmin = gulp.parallel(buildIdnetityServerAdminStyles, processIdentityServerAdminScripts);

gulp.task('cleanIdentityServerAdmin', processIdentityServerAdminClean);
gulp.task('stylesIdentityServerAdmin', buildIdnetityServerAdminStyles);
gulp.task('sassIdentityServerAdmin', processIdentityServerAdminSass);
gulp.task('sassIdentityServerAdmin:min', processIdentityServerAdminSassMin);
gulp.task('fontsIdentityServerAdmin', processIdentityServerAdminFonts);
gulp.task('scriptsIdentityServerAdmin', processIdentityServerAdminScripts);
gulp.task('buildIdentityServerAdmin', buildIdentityServerAdmin);
