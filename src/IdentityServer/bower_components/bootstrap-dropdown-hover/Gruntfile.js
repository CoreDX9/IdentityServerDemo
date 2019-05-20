module.exports = function (grunt) {

  grunt.initConfig({

    // Import package manifest
    pkg: grunt.file.readJSON("bootstrap-dropdown-hover.jquery.json"),

    // Banner definitions
    meta: {
      banner: "/*\n" +
        " *  <%= pkg.title || pkg.name %> - v<%= pkg.version %>\n" +
        " *  <%= pkg.description %>\n" +
        " *  <%= pkg.homepage %>\n" +
        " *\n" +
        " *  Made by <%= pkg.author.name %>\n" +
        " *  Under <%= pkg.licenses[0].type %> License\n" +
        " */\n"
    },

    // Concat definitions
    concat: {
      js: {
        src: ["src/jquery.bootstrap-dropdown-hover.js"],
        dest: "dist/jquery.bootstrap-dropdown-hover.js"
      },
      options: {
        banner: "<%= meta.banner %>"
      }
    },

    // Lint definitions
    jshint: {
      files: ["src/jquery.bootstrap-dropdown-hover.js"],
      options: {
        jshintrc: ".jshintrc"
      }
    },

    // Minify definitions
    uglify: {
      js: {
        src: ["dist/jquery.bootstrap-dropdown-hover.js"],
        dest: "dist/jquery.bootstrap-dropdown-hover.min.js"
      },
      options: {
        banner: "<%= meta.banner %>"
      }
    },

  });

  grunt.loadNpmTasks("grunt-contrib-concat");
  grunt.loadNpmTasks("grunt-contrib-jshint");
  grunt.loadNpmTasks("grunt-contrib-uglify");

  grunt.registerTask("default", ["jshint", "concat", "uglify"]);
  grunt.registerTask("travis", ["jshint"]);

};
