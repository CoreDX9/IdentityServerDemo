/*
 *  Bootstrap Dropdown Hover - v4.2.0
 *  Open dropdown menus on mouse hover, the proper way.
 *  http://www.virtuosoft.eu/code/bootstrap-dropdown-hover/
 *
 *  Made by István Ujj-Mészáros
 *  Under Apache License v2.0 License
 */
(function(factory) {
  if (typeof define === 'function' && define.amd) {
    define(['jquery'], factory);
  } else if (typeof module === 'object' && module.exports) {
    module.exports = function(root, jQuery) {
      if (jQuery === undefined) {
        if (typeof window !== 'undefined') {
          jQuery = require('jquery');
        }
        else {
          jQuery = require('jquery')(root);
        }
      }
      factory(jQuery);
      return jQuery;
    };
  } else {
    factory(jQuery);
  }
}(function($) {
  var pluginName = 'bootstrapDropdownHover',
      defaults = {
        clickBehavior: 'sticky',  // Click behavior setting:
                                  // 'default' means that the dropdown toggles on hover and on click too
                                  // 'disable' disables dropdown toggling with clicking when mouse is detected
                                  // 'link'    disabled dropdown toggling, but allows link clicks to go through
                                  //  'sticky' means if we click on an opened dropdown then it will not hide on
                                  //           mouseleave but on a second click only
        hideTimeout: 200
      },
      _hideTimeoutHandler,
      _hardOpened = false,
      _isTouchDevice = false;

  // The actual plugin constructor
  function BootstrapDropdownHover(element, options) {
    this.element = $(element);
    this.settings = $.extend({}, defaults, options, this.element.data());
    this._defaults = defaults;
    this._name = pluginName;
    this.init();
  }

  // https://stackoverflow.com/a/4819886/504270
  function isTouchDevice() {
    var prefixes = ' -webkit- -moz- -o- -ms- '.split(' ');
    var mq = function(query) {
      return window.matchMedia(query).matches;
    };

    if (('ontouchstart' in window) || window.DocumentTouch && document instanceof DocumentTouch) {
      return true;
    }

    // include the 'heartz' as a way to have a non matching MQ to help terminate the join
    // https://git.io/vznFH
    var query = ['(', prefixes.join('touch-enabled),('), 'heartz', ')'].join('');
    return mq(query);
  }

  _isTouchDevice = isTouchDevice();

  function getParent($this) {
    var selector = $this.attr('data-target');
    var $parent;

    if (!selector) {
      selector = $this.attr('href');
      selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, ''); //strip for ie7
    }

    $parent = selector && $(selector);

    if (!$parent || !$parent.length) {$parent = $this.parent();}

    return $parent;
  }

  function bindEvents(dropdown) {
    var $parent = getParent(dropdown.element);

    $('.dropdown-toggle, .dropdown-menu', $parent).on('mouseenter.dropdownhover', function () {
      if (_isTouchDevice) {
        return;
      }

      clearTimeout(_hideTimeoutHandler);
      if (!$parent.is('.open, .show')) {
        _hardOpened = false;
        dropdown.element.dropdown('toggle');
      }
    });

    $('.dropdown-toggle, .dropdown-menu', $parent).on('mouseleave.dropdownhover', function () {
      if (_isTouchDevice) {
        return;
      }

      if (_hardOpened) {
        return;
      }
      _hideTimeoutHandler = setTimeout(function () {
        if ($parent.is('.open, .show')) {
          dropdown.element.dropdown('toggle');
        }
      }, dropdown.settings.hideTimeout);
    });

    dropdown.element.on('click.dropdownhover', function (e) {
      if (dropdown.settings.clickBehavior !== 'link' && _isTouchDevice) {
        return;
      }

      switch(dropdown.settings.clickBehavior) {
        case 'default':
          return;
        case 'disable':
          e.preventDefault();
          e.stopImmediatePropagation();
          return;
        case 'link':
          e.stopImmediatePropagation();
          return;
        case 'sticky':
          if (_hardOpened) {
            _hardOpened = false;
          }
          else {
            _hardOpened = true;
            if ($parent.is('.open, .show')) {
              e.stopImmediatePropagation();
              e.preventDefault();
            }
          }
          return;
      }
    });
  }

  function removeEvents(dropdown) {
    var $parent = getParent(dropdown.element);
    $('.dropdown-toggle, .dropdown-menu', $parent).off('.dropdownhover');
    // seems that bootstrap binds the click handler twice after we reinitializing the plugin after a destroy...
    $('.dropdown-toggle, .dropdown-menu', $parent).off('.dropdown');
    dropdown.element.off('.dropdownhover');
    $('body').off('.dropdownhover');
  }

  BootstrapDropdownHover.prototype = {
    init: function () {
      this.setClickBehavior(this.settings.clickBehavior);
      this.setHideTimeout(this.settings.hideTimeout);
      bindEvents(this);
      return this.element;
    },
    setClickBehavior: function(value) {
      this.settings.clickBehavior = value;
      return this.element;
    },
    setHideTimeout: function(value) {
      this.settings.hideTimeout = value;
      return this.element;
    },
    destroy: function() {
      clearTimeout(_hideTimeoutHandler);
      removeEvents(this);
      this.element.data('plugin_' + pluginName, null);
      return this.element;
    }
  };

  // A really lightweight plugin wrapper around the constructor,
  // preventing against multiple instantiations
  $.fn[pluginName] = function (options) {
    var args = arguments;

    // Is the first parameter an object (options), or was omitted, instantiate a new instance of the plugin.
    if (options === undefined || typeof options === 'object') {
      // This allows the plugin to be called with $.fn.bootstrapDropdownHover();
      if (!$.contains(document, $(this)[0])) {
        $('[data-toggle="dropdown"]').each(function (index, item) {
          // For each nested select, instantiate the plugin
          $(item).bootstrapDropdownHover(options);
        });
      }
      return this.each(function () {
        // If this is not a select
        if (!$(this).hasClass('dropdown-toggle') || $(this).data('toggle') !== 'dropdown') {
          $('[data-toggle="dropdown"]', this).each(function (index, item) {
            // For each nested select, instantiate the plugin
            $(item).bootstrapDropdownHover(options);
          });
        } else if (!$.data(this, 'plugin_' + pluginName)) {
          // Only allow the plugin to be instantiated once so we check that the element has no plugin instantiation yet

          // if it has no instance, create a new one, pass options to our plugin constructor,
          // and store the plugin instance in the elements jQuery data object.
          $.data(this, 'plugin_' + pluginName, new BootstrapDropdownHover(this, options));
        }
      });

      // If the first parameter is a string and it doesn't start with an underscore or "contains" the `init`-function,
      // treat this as a call to a public method.
    } else if (typeof options === 'string' && options[0] !== '_' && options !== 'init') {

      // Cache the method call to make it possible to return a value
      var returns;

      this.each(function () {
        var instance = $.data(this, 'plugin_' + pluginName);
        // Tests that there's already a plugin-instance and checks that the requested public method exists
        if (instance instanceof BootstrapDropdownHover && typeof instance[options] === 'function') {
          // Call the method of our plugin instance, and pass it the supplied arguments.
          returns = instance[options].apply(instance, Array.prototype.slice.call(args, 1));
        }
      });

      // If the earlier cached method gives a value back return the value,
      // otherwise return this to preserve chainability.
      return returns !== undefined ? returns : this;
    }

  };

}));
