(function (global, factory) {
  if (typeof define === "function" && define.amd) {
    define("PNotifyAnimate", ["exports", "PNotify"], factory);
  } else if (typeof exports !== "undefined") {
    factory(exports, require("./PNotify"));
  } else {
    var mod = {
      exports: {}
    };
    factory(mod.exports, global.PNotify);
    global.PNotifyAnimate = mod.exports;
  }
})(this, function (exports, _PNotify) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });

  var _PNotify2 = _interopRequireDefault(_PNotify);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
    return typeof obj;
  } : function (obj) {
    return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
  };

  var _extends = Object.assign || function (target) {
    for (var i = 1; i < arguments.length; i++) {
      var source = arguments[i];

      for (var key in source) {
        if (Object.prototype.hasOwnProperty.call(source, key)) {
          target[key] = source[key];
        }
      }
    }

    return target;
  };

  function data() {
    return _extends({
      "_notice": null, // The PNotify notice.
      "_options": {} // The options for the notice.
    }, _PNotify2.default.modules.Animate.defaults);
  };

  var methods = {
    initModule: function initModule(options) {
      this.set(options);
      this.setUpAnimations();
    },
    update: function update() {
      this.setUpAnimations();
    },
    setUpAnimations: function setUpAnimations() {
      var _notice = this.get("_notice");
      var _options = this.get("_options");
      if (this.get("animate")) {
        _notice.set({ "animation": "none" });
        if (!_notice._animateIn) {
          _notice._animateIn = _notice.animateIn;
        }
        if (!_notice._animateOut) {
          _notice._animateOut = _notice.animateOut;
        }
        _notice.animateIn = this.animateIn.bind(this);
        _notice.animateOut = this.animateOut.bind(this);
        var animSpeed = 250;
        if (_options.animateSpeed === "slow") {
          animSpeed = 400;
        } else if (_options.animateSpeed === "fast") {
          animSpeed = 100;
        } else if (_options.animateSpeed > 0) {
          animSpeed = _options.animateSpeed;
        }
        animSpeed = animSpeed / 1000;
        _notice.refs.elem.style.WebkitAnimationDuration = animSpeed + "s";
        _notice.refs.elem.style.MozAnimationDuration = animSpeed + "s";
        _notice.refs.elem.style.animationDuration = animSpeed + "s";
      } else if (_notice._animateIn && _notice._animateOut) {
        _notice.animateIn = _notice._animateIn;
        delete _notice._animateIn;
        _notice.animateOut = _notice._animateOut;
        delete _notice._animateOut;
      }
    },
    animateIn: function animateIn(callback) {
      var _notice = this.get("_notice");
      var _options = this.get("_options");
      // Declare that the notice is animating in.
      _notice.set({ "_animating": "in" });

      var finished = function finished() {
        _notice.refs.elem.removeEventListener('webkitAnimationEnd', finished);
        _notice.refs.elem.removeEventListener('mozAnimationEnd', finished);
        _notice.refs.elem.removeEventListener('MSAnimationEnd', finished);
        _notice.refs.elem.removeEventListener('oanimationend', finished);
        _notice.refs.elem.removeEventListener('animationend', finished);
        _notice.set({ "_animatingClass": "ui-pnotify-in animated" });
        if (callback) {
          callback.call();
        }
        // Declare that the notice has completed animating.
        _notice.set({ "_animating": false });
      };

      _notice.refs.elem.addEventListener('webkitAnimationEnd', finished);
      _notice.refs.elem.addEventListener('mozAnimationEnd', finished);
      _notice.refs.elem.addEventListener('MSAnimationEnd', finished);
      _notice.refs.elem.addEventListener('oanimationend', finished);
      _notice.refs.elem.addEventListener('animationend', finished);
      _notice.set({ "_animatingClass": "ui-pnotify-in animated " + this.get("inClass") });
    },
    animateOut: function animateOut(callback) {
      var _notice = this.get("_notice");
      var _options = this.get("_options");
      // Declare that the notice is animating out.
      _notice.set({ "_animating": "out" });

      var finished = function finished() {
        _notice.refs.elem.removeEventListener('webkitAnimationEnd', finished);
        _notice.refs.elem.removeEventListener('mozAnimationEnd', finished);
        _notice.refs.elem.removeEventListener('MSAnimationEnd', finished);
        _notice.refs.elem.removeEventListener('oanimationend', finished);
        _notice.refs.elem.removeEventListener('animationend', finished);
        _notice.set({ "_animatingClass": "animated" });
        if (callback) {
          callback.call();
        }
        // Declare that the notice has completed animating.
        _notice.set({ "_animating": false });
      };

      _notice.refs.elem.addEventListener('webkitAnimationEnd', finished);
      _notice.refs.elem.addEventListener('mozAnimationEnd', finished);
      _notice.refs.elem.addEventListener('MSAnimationEnd', finished);
      _notice.refs.elem.addEventListener('oanimationend', finished);
      _notice.refs.elem.addEventListener('animationend', finished);
      _notice.set({ "_animatingClass": "ui-pnotify-in animated " + this.get("outClass") });
    }
  };

  function setup(Component) {
    Component.key = "Animate";

    Component.defaults = {
      // Use animate.css to animate the notice.
      animate: false,
      // The class to use to animate the notice in.
      inClass: "",
      // The class to use to animate the notice out.
      outClass: ""
    };

    Component.init = function (notice) {
      notice.attention = function (aniClass, callback) {
        var cb = function cb() {
          notice.refs.container.removeEventListener('webkitAnimationEnd', cb);
          notice.refs.container.removeEventListener('mozAnimationEnd', cb);
          notice.refs.container.removeEventListener('MSAnimationEnd', cb);
          notice.refs.container.removeEventListener('oanimationend', cb);
          notice.refs.container.removeEventListener('animationend', cb);
          notice.refs.container.classList.remove(aniClass);
          if (callback) {
            callback.call(notice);
          }
        };
        notice.refs.container.addEventListener('webkitAnimationEnd', cb);
        notice.refs.container.addEventListener('mozAnimationEnd', cb);
        notice.refs.container.addEventListener('MSAnimationEnd', cb);
        notice.refs.container.addEventListener('oanimationend', cb);
        notice.refs.container.addEventListener('animationend', cb);
        notice.refs.container.classList.add("animated");
        notice.refs.container.classList.add(aniClass);
      };

      return new Component({ target: document.body });
    };

    // Register the module with PNotify.
    _PNotify2.default.modules.Animate = Component;
  };

  function create_main_fragment(state, component) {

    return {
      c: noop,

      m: noop,

      p: noop,

      u: noop,

      d: noop
    };
  }

  function PNotifyAnimate(options) {
    init(this, options);
    this._state = assign(data(), options.data);

    this._fragment = create_main_fragment(this._state, this);

    if (options.target) {
      this._fragment.c();
      this._fragment.m(options.target, options.anchor || null);
    }
  }

  assign(PNotifyAnimate.prototype, methods, {
    destroy: destroy,
    get: get,
    fire: fire,
    observe: observe,
    on: on,
    set: set,
    teardown: destroy,
    _set: _set,
    _mount: _mount,
    _unmount: _unmount,
    _differs: _differs
  });

  PNotifyAnimate.prototype._recompute = noop;

  setup(PNotifyAnimate);

  function noop() {}

  function init(component, options) {
    component._observers = { pre: blankObject(), post: blankObject() };
    component._handlers = blankObject();
    component._bind = options._bind;

    component.options = options;
    component.root = options.root || component;
    component.store = component.root.store || options.store;
  }

  function assign(target) {
    var k,
        source,
        i = 1,
        len = arguments.length;
    for (; i < len; i++) {
      source = arguments[i];
      for (k in source) {
        target[k] = source[k];
      }
    }

    return target;
  }

  function destroy(detach) {
    this.destroy = noop;
    this.fire('destroy');
    this.set = this.get = noop;

    if (detach !== false) this._fragment.u();
    this._fragment.d();
    this._fragment = this._state = null;
  }

  function get(key) {
    return key ? this._state[key] : this._state;
  }

  function fire(eventName, data) {
    var handlers = eventName in this._handlers && this._handlers[eventName].slice();
    if (!handlers) return;

    for (var i = 0; i < handlers.length; i += 1) {
      handlers[i].call(this, data);
    }
  }

  function observe(key, callback, options) {
    var group = options && options.defer ? this._observers.post : this._observers.pre;

    (group[key] || (group[key] = [])).push(callback);

    if (!options || options.init !== false) {
      callback.__calling = true;
      callback.call(this, this._state[key]);
      callback.__calling = false;
    }

    return {
      cancel: function cancel() {
        var index = group[key].indexOf(callback);
        if (~index) group[key].splice(index, 1);
      }
    };
  }

  function on(eventName, handler) {
    if (eventName === 'teardown') return this.on('destroy', handler);

    var handlers = this._handlers[eventName] || (this._handlers[eventName] = []);
    handlers.push(handler);

    return {
      cancel: function cancel() {
        var index = handlers.indexOf(handler);
        if (~index) handlers.splice(index, 1);
      }
    };
  }

  function set(newState) {
    this._set(assign({}, newState));
    if (this.root._lock) return;
    this.root._lock = true;
    callAll(this.root._beforecreate);
    callAll(this.root._oncreate);
    callAll(this.root._aftercreate);
    this.root._lock = false;
  }

  function _set(newState) {
    var oldState = this._state,
        changed = {},
        dirty = false;

    for (var key in newState) {
      if (this._differs(newState[key], oldState[key])) changed[key] = dirty = true;
    }
    if (!dirty) return;

    this._state = assign({}, oldState, newState);
    this._recompute(changed, this._state);
    if (this._bind) this._bind(changed, this._state);

    if (this._fragment) {
      dispatchObservers(this, this._observers.pre, changed, this._state, oldState);
      this._fragment.p(changed, this._state);
      dispatchObservers(this, this._observers.post, changed, this._state, oldState);
    }
  }

  function _mount(target, anchor) {
    this._fragment.m(target, anchor);
  }

  function _unmount() {
    if (this._fragment) this._fragment.u();
  }

  function _differs(a, b) {
    return a != a ? b == b : a !== b || a && (typeof a === "undefined" ? "undefined" : _typeof(a)) === 'object' || typeof a === 'function';
  }

  function blankObject() {
    return Object.create(null);
  }

  function callAll(fns) {
    while (fns && fns.length) {
      fns.shift()();
    }
  }

  function dispatchObservers(component, group, changed, newState, oldState) {
    for (var key in group) {
      if (!changed[key]) continue;

      var newValue = newState[key];
      var oldValue = oldState[key];

      var callbacks = group[key];
      if (!callbacks) continue;

      for (var i = 0; i < callbacks.length; i += 1) {
        var callback = callbacks[i];
        if (callback.__calling) continue;

        callback.__calling = true;
        callback.call(component, newValue, oldValue);
        callback.__calling = false;
      }
    }
  }
  exports.default = PNotifyAnimate;
});