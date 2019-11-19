# Bootstrap Dropdown Hover [![Build Status](https://secure.travis-ci.org/istvan-ujjmeszaros/bootstrap-dropdown-hover.png?branch=master)](https://travis-ci.org/istvan-ujjmeszaros/bootstrap-dropdown-hover)
Bootstrap Dropdown Hover is a simple plugin which opens Bootstrap dropdown menus on mouse hover, the proper way.

## Demo

Check the [official website](http://www.virtuosoft.eu/code/bootstrap-dropdown-hover/) for a demo.

## Why I made it, when there are many solutions already?

I had issues with all the previously existing solutions. The simple CSS ones are not using the `.open` class on the parent element, so there will be no feedback on the dropdown toggle element when the dropdown menu is visible.

The js ones are interfering with clicking on `.dropdown-toggle`, so the dropdown menu shows up on hover, then it is hiding when clicking on the `.dropdown-toggle` element, and moving out the mouse will trigger the dropdown menu to show up again.
Some of the js solutions are braking iOS compatibility, some plugins are not working on modern desktop browsers which are supporting the touch events.

That's why I made this proper plugin, which prevents all these issues by using only the standard Bootstrap javascript API, without any hack.

## Usage

1. Download the latest tag from the [releases page](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/releases) or get it via **bower**:

```shell
$ bower install bootstrap-dropdown-hover
```

2. Include **jQuery** and **Bootstrap**:

```html
<link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
<script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
```

3. Include plugin's code:

```html
<script src="dist/jquery.bootstrap-dropdown-hover.min.js"></script>
```

4. Call the plugin:

Initiate on all dropdowns/dropups method 1

```javascript
$.fn.bootstrapDropdownHover({
  // see next for specifications
});
```

Initiate on all dropdowns/dropups method 1

```javascript
$('[data-toggle="dropdown"]').bootstrapDropdownHover({
  // see next for specifications
});
```

Initiate on navbar menu only

```javascript
$('.navbar [data-toggle="dropdown"]').bootstrapDropdownHover({
  // see next for specifications
});
```

## Specifications

### Initialization parameters object

When calling `bootstrapDropdownHover()` you can pass a parameters object with zero or more of the following:

- `clickBehavior`, `'sticky'`|`'default'`|`'disable'`|`'link'`, defaults to `sticky`, which means that if we click on an opened dropdown then it will not hide on mouseleave but on a second click only. Can be `default`, which means that means that the dropdown toggles on hover and on click too, `disable`, which disables dropdown toggling with clicking when mouse is detected (so it will open on hover only) or `link` which is like `disable`, but does not prevent the default browser action (i.e. link clicks).
- `hideTimeout`, integer, defaults to `200`, how much time the hovered dropdown hides after mouseleave (in milliseconds).

These settings can also be set via HTML5 data attributes on the element itself, e.g. this will
override both default and explicit constructor settings:
```html
<a data-toggle="dropdown" data-click-behavior="default"></a>
```

### Methods

You can modify the behavior of the plugin by calling its methods, all of which accept a `value`.

To call methods on any dropdown hover instance, use the following syntax:

```javascript
$(selector).bootstrapDropdownHover(methodName, parameter);
```

Here are the available methods:

- `setClickBehavior(value)` to change the `clickBehavior` parameter.
- `setHideTimeout(value)` to change the `hideTimeout` parameter.

Furthermore, you can call:

- `destroy()` to restore the original behavior.

## Structure

The basic structure of the project is given in the following way:

```
├── dist/
│   ├── jquery.bootstrap-dropdown-hover.js
│   └── jquery.bootstrap-dropdown-hover.min.js
├── src/
│   └── jquery.bootstrap-dropdown-hover.js
├── .editorconfig
├── .gitignore
├── .jshintrc
├── .travis.yml
├── bootstrap-dropdown-hover.jquery.json
├── bower.json
├── Gruntfile.js
├── index.html
└── package.json
```

#### [dist/](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/dist)

This is where the generated files are stored once Grunt runs.

#### [src/](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/src)

Contains the source file.

#### [.editorconfig](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/.editorconfig)

This file is for unifying the coding style for different editors and IDEs.

> Check [editorconfig.org](http://editorconfig.org) if you haven't heard about this project yet.

#### [.gitignore](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/.gitignore)

List of files that we don't want Git to track.

> Check this [Git Ignoring Files Guide](https://help.github.com/articles/ignoring-files) for more details.

#### [.jshintrc](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/.jshintrc)

List of rules used by JSHint to detect errors and potential problems in JavaScript.

> Check [jshint.com](http://jshint.com/about/) if you haven't heard about this project yet.

#### [.travis.yml](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/.travis.yml)

Definitions for continous integration using Travis.

> Check [travis-ci.org](http://about.travis-ci.org/) if you haven't heard about this project yet.

#### [bootstrap-dropdown-hover.jquery.json](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/bootstrap-dropdown-hover.jquery.json)

Package manifest file used to publish plugins in jQuery Plugin Registry.

> Check this [Package Manifest Guide](http://plugins.jquery.com/docs/package-manifest/) for more details.

#### [Gruntfile.js](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/Gruntfile.js)

Contains all automated tasks using Grunt.

> Check [gruntjs.com](http://gruntjs.com) if you haven't heard about this project yet.

#### [package.json](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/tree/master/package.json)

Specify all dependencies loaded via Node.JS.

> Check [NPM](https://npmjs.org/doc/json.html) for more details.

## Building

To build and test the plugin, you need:

- [**NodeJS**](www.nodejs.org) with **npm**
- **bower** (install it with `npm install bower --g`)
- **grunt-cli** (install it with `npm install grunt-cli --g`)

Then, `cd` to the project directory and install the required dependencies:

```shell
$ npm install
$ bower install
```

To run jshint on the plugin code, call:

```shell
$ grunt jshint
```

To build the output js and css files, with the related minified ones, run:

```shell
$ grunt
```

## Issues and Contributions

You can report any issue you may encounter on the [GitHub Issue Tracker](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/issues).

To contribute, please follow the [contribution guidelines](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/blob/master/CONTRIBUTING.md).

## History

Check [Release](https://github.com/istvan-ujjmeszaros/bootstrap-dropdown-hover/releases) list.

## License

```
  Copyright 2015 István Ujj-Mészáros

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
```