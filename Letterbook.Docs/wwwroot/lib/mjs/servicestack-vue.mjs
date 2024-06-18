var Ys = Object.defineProperty;
var eo = (e, t, l) => t in e ? Ys(e, t, { enumerable: !0, configurable: !0, writable: !0, value: l }) : e[t] = l;
var _e = (e, t, l) => (eo(e, typeof t != "symbol" ? t + "" : t, l), l);
import { defineComponent as ue, computed as f, openBlock as a, createElementBlock as u, normalizeClass as w, unref as o, createElementVNode as s, createCommentVNode as k, renderSlot as Z, ref as M, toDisplayString as A, inject as qe, nextTick as xt, isRef as qn, mergeProps as Le, withModifiers as Ne, h as ht, resolveComponent as X, createBlock as se, withCtx as ke, useAttrs as to, createVNode as $e, createTextVNode as xe, watchEffect as hl, normalizeStyle as Jl, Fragment as Te, renderList as Ie, withDirectives as kt, vModelCheckbox as Xl, withKeys as Qn, createStaticVNode as wl, vModelSelect as lo, useSlots as Yl, getCurrentInstance as Be, onMounted as Xe, createSlots as en, normalizeProps as It, guardReactiveProps as gl, vModelDynamic as no, onUnmounted as Pt, watch as Lt, vModelText as so, resolveDynamicComponent as Kn, provide as Xt, resolveDirective as oo } from "vue";
import { errorResponseExcept as ao, dateFmt as Zn, toTime as io, omit as ut, enc as Hl, setQueryString as ro, appendQueryString as Kt, nameOf as uo, ApiResult as We, lastRightPart as $t, leftPart as xl, map as Ue, toDate as Vt, toDateTime as co, toCamelCase as fo, mapGet as be, chop as mo, fromXsdDuration as Gn, isDate as kl, timeFmt12 as vo, apiValue as ho, indexOfAny as go, createBus as po, toKebabCase as Rn, humanize as Ee, delaySet as Wn, rightPart as dl, queryString as El, combinePaths as yo, toPascalCase as nt, errorResponse as ft, trimEnd as bo, $1 as pl, lastLeftPart as wo, ResponseStatus as Pl, ResponseError as Hn, HttpMethods as tn, uniqueKeys as zl, humanify as Jn, each as xo } from "@servicestack/client";
const ko = { class: "flex items-center" }, $o = {
  key: 0,
  class: "flex-shrink-0 mr-3"
}, Co = {
  key: 0,
  class: "h-5 w-5 text-yellow-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, _o = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z",
  "clip-rule": "evenodd"
}, null, -1), Lo = [
  _o
], Vo = {
  key: 1,
  class: "h-5 w-5 text-red-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, So = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z",
  "clip-rule": "evenodd"
}, null, -1), Mo = [
  So
], Ao = {
  key: 2,
  class: "h-5 w-5 text-blue-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, To = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M19 10.5a8.5 8.5 0 11-17 0 8.5 8.5 0 0117 0zM8.25 9.75A.75.75 0 019 9h.253a1.75 1.75 0 011.709 2.13l-.46 2.066a.25.25 0 00.245.304H11a.75.75 0 010 1.5h-.253a1.75 1.75 0 01-1.709-2.13l.46-2.066a.25.25 0 00-.245-.304H9a.75.75 0 01-.75-.75zM10 7a1 1 0 100-2 1 1 0 000 2z",
  "clip-rule": "evenodd"
}, null, -1), Fo = [
  To
], Io = {
  key: 3,
  class: "h-5 w-5 text-green-400",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Do = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z",
  "clip-rule": "evenodd"
}, null, -1), Oo = [
  Do
], jo = /* @__PURE__ */ ue({
  __name: "Alert",
  props: {
    type: { default: "warn" },
    hideIcon: { type: Boolean }
  },
  setup(e) {
    const t = e, l = f(() => t.type == "info" ? "bg-blue-50 dark:bg-blue-200" : t.type == "error" ? "bg-red-50 dark:bg-red-200" : t.type == "success" ? "bg-green-50 dark:bg-green-200" : "bg-yellow-50 dark:bg-yellow-200"), n = f(() => t.type == "info" ? "border-blue-400" : t.type == "error" ? "border-red-400" : t.type == "success" ? "border-green-400" : "border-yellow-400"), i = f(() => t.type == "info" ? "text-blue-700" : t.type == "error" ? "text-red-700" : t.type == "success" ? "text-green-700" : "text-yellow-700");
    return (r, d) => (a(), u("div", {
      class: w([o(l), o(n), "border-l-4 p-4"])
    }, [
      s("div", ko, [
        e.hideIcon ? k("", !0) : (a(), u("div", $o, [
          e.type == "warn" ? (a(), u("svg", Co, Lo)) : e.type == "error" ? (a(), u("svg", Vo, Mo)) : e.type == "info" ? (a(), u("svg", Ao, Fo)) : e.type == "success" ? (a(), u("svg", Io, Oo)) : k("", !0)
        ])),
        s("div", null, [
          s("p", {
            class: w([o(i), "text-sm"])
          }, [
            Z(r.$slots, "default")
          ], 2)
        ])
      ])
    ], 2));
  }
}), Po = {
  key: 0,
  class: "rounded-md bg-green-50 dark:bg-green-200 p-4",
  role: "alert"
}, Bo = { class: "flex" }, Ro = /* @__PURE__ */ s("div", { class: "flex-shrink-0" }, [
  /* @__PURE__ */ s("svg", {
    class: "h-5 w-5 text-green-400 dark:text-green-500",
    fill: "none",
    stroke: "currentColor",
    viewBox: "0 0 24 24",
    xmlns: "http://www.w3.org/2000/svg"
  }, [
    /* @__PURE__ */ s("path", {
      "stroke-linecap": "round",
      "stroke-linejoin": "round",
      "stroke-width": "2",
      d: "M5 13l4 4L19 7"
    })
  ])
], -1), Ho = { class: "ml-3" }, Eo = { class: "text-sm font-medium text-green-800" }, zo = { key: 0 }, No = { class: "ml-auto pl-3" }, Uo = { class: "-mx-1.5 -my-1.5" }, qo = /* @__PURE__ */ s("span", { class: "sr-only" }, "Dismiss", -1), Qo = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", { d: "M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" })
], -1), Ko = [
  qo,
  Qo
], Zo = /* @__PURE__ */ ue({
  __name: "AlertSuccess",
  props: {
    message: null
  },
  setup(e) {
    const t = M(!1);
    return (l, n) => t.value ? k("", !0) : (a(), u("div", Po, [
      s("div", Bo, [
        Ro,
        s("div", Ho, [
          s("h3", Eo, [
            e.message ? (a(), u("span", zo, A(e.message), 1)) : Z(l.$slots, "default", { key: 1 })
          ])
        ]),
        s("div", No, [
          s("div", Uo, [
            s("button", {
              type: "button",
              class: "inline-flex rounded-md bg-green-50 dark:bg-green-200 p-1.5 text-green-500 dark:text-green-600 hover:bg-green-100 dark:hover:bg-green-700 dark:hover:text-white focus:outline-none focus:ring-2 focus:ring-green-600 focus:ring-offset-2 focus:ring-offset-green-50 dark:ring-offset-green-200",
              onClick: n[0] || (n[0] = (i) => t.value = !0)
            }, Ko)
          ])
        ])
      ])
    ]));
  }
}), Go = { class: "flex" }, Wo = /* @__PURE__ */ s("div", { class: "flex-shrink-0" }, [
  /* @__PURE__ */ s("svg", {
    class: "h-5 w-5 text-red-400",
    xmlns: "http://www.w3.org/2000/svg",
    viewBox: "0 0 24 24"
  }, [
    /* @__PURE__ */ s("path", {
      fill: "currentColor",
      d: "M12 2c5.53 0 10 4.47 10 10s-4.47 10-10 10S2 17.53 2 12S6.47 2 12 2m3.59 5L12 10.59L8.41 7L7 8.41L10.59 12L7 15.59L8.41 17L12 13.41L15.59 17L17 15.59L13.41 12L17 8.41L15.59 7Z"
    })
  ])
], -1), Jo = { class: "ml-3" }, Xo = { class: "text-sm text-red-700 dark:text-red-200" }, Yo = /* @__PURE__ */ ue({
  __name: "ErrorSummary",
  props: {
    status: null,
    except: null,
    class: null
  },
  setup(e) {
    const t = e;
    let l = qe("ApiState", void 0);
    const n = f(() => t.status || l != null && l.error.value ? ao.call({ responseStatus: t.status ?? (l == null ? void 0 : l.error.value) }, t.except ?? []) : null);
    return (i, r) => o(n) ? (a(), u("div", {
      key: 0,
      class: w(`bg-red-50 dark:bg-red-900 border-l-4 border-red-400 p-4 ${i.$props.class}`)
    }, [
      s("div", Go, [
        Wo,
        s("div", Jo, [
          s("p", Xo, A(o(n)), 1)
        ])
      ])
    ], 2)) : k("", !0);
  }
}), ea = ["id", "aria-describedby"], ta = /* @__PURE__ */ ue({
  __name: "InputDescription",
  props: {
    id: null,
    description: null
  },
  setup(e) {
    return (t, l) => e.description ? (a(), u("div", {
      key: "description",
      class: "mt-2 text-sm text-gray-500",
      id: `${e.id}-description`,
      "aria-describedby": `${e.id}-description`
    }, [
      s("div", null, A(e.description), 1)
    ], 8, ea)) : k("", !0);
  }
});
function $l(e) {
  return Zn(e).replace(/\//g, "-");
}
function Xn(e) {
  return e == null ? "" : io(e);
}
function Yn(e, t) {
  e.value = null, xt(() => e.value = t);
}
function At(e) {
  return Object.keys(e).forEach((t) => {
    const l = e[t];
    e[t] = qn(l) ? o(l) : l;
  }), e;
}
function Ct(e, t, l) {
  l ? (t.value = e.entering.cls + " " + e.entering.from, setTimeout(() => t.value = e.entering.cls + " " + e.entering.to, 0)) : (t.value = e.leaving.cls + " " + e.leaving.from, setTimeout(() => t.value = e.leaving.cls + " " + e.leaving.to, 0));
}
function cl(e) {
  if (typeof document > "u")
    return;
  let t = (e == null ? void 0 : e.after) || document.activeElement, l = t && t.form;
  if (l) {
    let n = ':not([disabled]):not([tabindex="-1"])', i = l.querySelectorAll(`a:not([disabled]), button${n}, input[type=text]${n}, [tabindex]${n}`), r = Array.prototype.filter.call(
      i,
      (c) => c.offsetWidth > 0 || c.offsetHeight > 0 || c === t
    ), d = r.indexOf(t);
    d > -1 && (r[d + 1] || r[0]).focus();
  }
}
function Bt(e) {
  if (!e)
    return null;
  if (typeof e == "string")
    return e;
  const t = typeof e == "function" ? new e() : typeof e == "object" ? e : null;
  if (!t)
    throw new Error(`Invalid DTO Type '${typeof e}'`);
  if (typeof t.getTypeName != "function")
    throw new Error(`${JSON.stringify(t)} is not a Request DTO`);
  const l = t.getTypeName();
  if (!l)
    throw new Error("DTO Required");
  return l;
}
function it(e, t, l) {
  l || (l = {});
  let n = l.cls || l.className || l.class;
  return n && (l = ut(l, ["cls", "class", "className"]), l.class = n), t == null ? `<${e}` + Nl(l) + "/>" : `<${e}` + Nl(l) + `>${t || ""}</${e}>`;
}
function Nl(e) {
  return Object.keys(e).reduce((t, l) => `${t} ${l}="${Hl(e[l])}"`, "");
}
function Cl(e) {
  return Object.assign({ target: "_blank", rel: "noopener", class: "text-blue-600" }, e);
}
function Ft(e) {
  return wn(e);
}
let la = ["string", "number", "boolean", "null", "undefined"];
function _t(e) {
  return la.indexOf(typeof e) >= 0 || e instanceof Date;
}
function Zt(e) {
  return !_t(e);
}
class es {
  get length() {
    return typeof localStorage > "u" ? 0 : localStorage.length;
  }
  getItem(t) {
    return typeof localStorage > "u" ? null : localStorage.getItem(t);
  }
  setItem(t, l) {
    typeof localStorage > "u" || localStorage.setItem(t, l);
  }
  removeItem(t) {
    typeof localStorage > "u" || localStorage.removeItem(t);
  }
  clear() {
    typeof localStorage > "u" || localStorage.clear();
  }
  key(t) {
    return typeof localStorage > "u" ? null : localStorage.key(t);
  }
}
function yl(e) {
  return typeof e == "string" ? JSON.parse(e) : null;
}
function ln(e) {
  if (typeof history < "u") {
    const t = ro(location.href, e);
    history.pushState({}, "", t);
  }
}
function nn(e, t) {
  if (["function", "Function", "eval", "=>", ";"].some((i) => e.includes(i)))
    throw new Error(`Unsafe script: '${e}'`);
  const n = Object.assign(
    Object.keys(globalThis).reduce((i, r) => (i[r] = void 0, i), {}),
    t
  );
  return new Function("with(this) { return (" + e + ") }").call(n);
}
function Ul(e) {
  typeof navigator < "u" && navigator.clipboard.writeText(e);
}
function sn(e) {
  const t = ie.config.storage.getItem(e);
  return t ? JSON.parse(t) : null;
}
function _l(e, t) {
  return Kt(`swr.${uo(e)}`, t ? Object.assign({}, e, t) : e);
}
function na(e) {
  if (e.request) {
    const t = _l(e.request, e.args);
    ie.config.storage.removeItem(t);
  }
}
async function ts(e, t, l, n, i) {
  const r = _l(t, n);
  l(new We({ response: sn(r) }));
  const d = await e.api(t, n, i);
  if (d.succeeded && d.response) {
    d.response._date = new Date().valueOf();
    const c = JSON.stringify(d.response);
    ie.config.storage.setItem(r, c), l(d);
  }
  return d;
}
function ls(e, t) {
  let l = null;
  return (...n) => {
    l && clearTimeout(l), l = setTimeout(() => {
      e(...n);
    }, t || 100);
  };
}
function gt(e) {
  return typeof e == "string" ? e.split(",") : e || [];
}
function bt(e, t) {
  const l = gt(t);
  return e.reduce((n, i) => (n[i] = !l.includes(i), n), {});
}
function ns() {
  return {
    LocalStore: es,
    dateInputFormat: $l,
    timeInputFormat: Xn,
    setRef: Yn,
    unRefs: At,
    transition: Ct,
    focusNextElement: cl,
    getTypeName: Bt,
    htmlTag: it,
    htmlAttrs: Nl,
    linkAttrs: Cl,
    toAppUrl: Ft,
    isPrimitive: _t,
    isComplexType: Zt,
    pushState: ln,
    scopedExpr: nn,
    copyText: Ul,
    fromCache: sn,
    swrCacheKey: _l,
    swrClear: na,
    swrApi: ts,
    asStrings: gt,
    asOptions: bt,
    createDebounce: ls
  };
}
const ss = "png,jpg,jpeg,jfif,gif,svg,webp".split(","), os = {
  img: "png,jpg,jpeg,gif,svg,webp,png,jpg,jpeg,gif,bmp,tif,tiff,webp,ai,psd,ps".split(","),
  vid: "avi,m4v,mov,mp4,mpg,mpeg,wmv,webm".split(","),
  aud: "mp3,mpa,ogg,wav,wma,mid,webm".split(","),
  ppt: "key,odp,pps,ppt,pptx".split(","),
  xls: "xls,xlsm,xlsx,ods,csv,tsv".split(","),
  doc: "doc,docx,pdf,rtf,tex,txt,md,rst,xls,xlsm,xlsx,ods,key,odp,pps,ppt,pptx".split(","),
  zip: "zip,tar,gz,7z,rar,gzip,deflate,br,iso,dmg,z,lz,lz4,lzh,s7z,apl,arg,jar,war".split(","),
  exe: "exe,bat,sh,cmd,com,app,msi,run,vb,vbs,js,ws,wsh".split(","),
  att: "bin,oct,dat".split(",")
  //attachment
}, En = Object.keys(os), ct = (e, t) => `<svg xmlns='http://www.w3.org/2000/svg' aria-hidden='true' role='img' preserveAspectRatio='xMidYMid meet' viewBox='${e}'>${t}</svg>`, fl = {
  img: ct("4 4 16 16", "<path fill='currentColor' d='M20 6v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2zm-2 0H6v6.38l2.19-2.19l5.23 5.23l1-1a1.59 1.59 0 0 1 2.11.11L18 16V6zm-5 3.5a1.5 1.5 0 1 1 3 0a1.5 1.5 0 0 1-3 0z'/>"),
  vid: ct("0 0 24 24", "<path fill='currentColor' d='m14 2l6 6v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h8m4 18V9h-5V4H6v16h12m-2-2l-2.5-1.7V18H8v-5h5.5v1.7L16 13v5Z'/>"),
  aud: ct("0 0 24 24", "<path fill='currentColor' d='M14 2H6c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V8l-6-6zM6 20V4h7v5h5v11H6zm10-9h-4v3.88a2.247 2.247 0 0 0-3.5 1.87c0 1.24 1.01 2.25 2.25 2.25S13 17.99 13 16.75V13h3v-2z'/>"),
  ppt: ct("0 0 48 48", "<g fill='none' stroke='currentColor' stroke-linecap='round' stroke-linejoin='round' stroke-width='4'><path d='M4 8h40'/><path d='M8 8h32v26H8V8Z' clip-rule='evenodd'/><path d='m22 16l5 5l-5 5m-6 16l8-8l8 8'/></g>"),
  xls: ct("0 0 256 256", "<path fill='currentColor' d='M200 26H72a14 14 0 0 0-14 14v26H40a14 14 0 0 0-14 14v96a14 14 0 0 0 14 14h18v26a14 14 0 0 0 14 14h128a14 14 0 0 0 14-14V40a14 14 0 0 0-14-14Zm-42 76h44v52h-44Zm44-62v50h-44V80a14 14 0 0 0-14-14h-2V38h58a2 2 0 0 1 2 2ZM70 40a2 2 0 0 1 2-2h58v28H70ZM38 176V80a2 2 0 0 1 2-2h104a2 2 0 0 1 2 2v96a2 2 0 0 1-2 2H40a2 2 0 0 1-2-2Zm32 40v-26h60v28H72a2 2 0 0 1-2-2Zm130 2h-58v-28h2a14 14 0 0 0 14-14v-10h44v50a2 2 0 0 1-2 2ZM69.2 148.4L84.5 128l-15.3-20.4a6 6 0 1 1 9.6-7.2L92 118l13.2-17.6a6 6 0 0 1 9.6 7.2L99.5 128l15.3 20.4a6 6 0 0 1-9.6 7.2L92 138l-13.2 17.6a6 6 0 1 1-9.6-7.2Z'/>"),
  doc: ct("0 0 32 32", "<path fill='currentColor' d='M26 30H11a2.002 2.002 0 0 1-2-2v-6h2v6h15V6h-9V4h9a2.002 2.002 0 0 1 2 2v22a2.002 2.002 0 0 1-2 2Z'/><path fill='currentColor' d='M17 10h7v2h-7zm-1 5h8v2h-8zm-1 5h9v2h-9zm-6-1a5.005 5.005 0 0 1-5-5V3h2v11a3 3 0 0 0 6 0V5a1 1 0 0 0-2 0v10H8V5a3 3 0 0 1 6 0v9a5.005 5.005 0 0 1-5 5z'/>"),
  zip: ct("0 0 16 16", "<g fill='currentColor'><path d='M6.5 7.5a1 1 0 0 1 1-1h1a1 1 0 0 1 1 1v.938l.4 1.599a1 1 0 0 1-.416 1.074l-.93.62a1 1 0 0 1-1.109 0l-.93-.62a1 1 0 0 1-.415-1.074l.4-1.599V7.5zm2 0h-1v.938a1 1 0 0 1-.03.243l-.4 1.598l.93.62l.93-.62l-.4-1.598a1 1 0 0 1-.03-.243V7.5z'/><path d='M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2zm5.5-1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H9v1H8v1h1v1H8v1h1v1H7.5V5h-1V4h1V3h-1V2h1V1z'/></g>"),
  exe: ct("0 0 16 16", "<path fill='currentColor' fill-rule='evenodd' d='M14 4.5V14a2 2 0 0 1-2 2h-1v-1h1a1 1 0 0 0 1-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5L14 4.5ZM2.575 15.202H.785v-1.073H2.47v-.606H.785v-1.025h1.79v-.648H0v3.999h2.575v-.647ZM6.31 11.85h-.893l-.823 1.439h-.036l-.832-1.439h-.931l1.227 1.983l-1.239 2.016h.861l.853-1.415h.035l.85 1.415h.908l-1.254-1.992L6.31 11.85Zm1.025 3.352h1.79v.647H6.548V11.85h2.576v.648h-1.79v1.025h1.684v.606H7.334v1.073Z'/>"),
  att: ct("0 0 24 24", "<path fill='currentColor' d='M14 0a5 5 0 0 1 5 5v12a7 7 0 1 1-14 0V9h2v8a5 5 0 0 0 10 0V5a3 3 0 1 0-6 0v12a1 1 0 1 0 2 0V6h2v11a3 3 0 1 1-6 0V5a5 5 0 0 1 5-5Z'/>")
}, sa = /[\r\n%#()<>?[\\\]^`{|}]/g, zn = 1024, oa = ["Bytes", "KB", "MB", "GB", "TB"], aa = (() => {
  const e = "application/", t = e + "vnd.openxmlformats-officedocument.", l = "image/", n = "text/", i = "audio/", r = "video/", d = {
    jpg: l + "jpeg",
    tif: l + "tiff",
    svg: l + "svg+xml",
    ico: l + "x-icon",
    ts: n + "typescript",
    py: n + "x-python",
    sh: n + "x-sh",
    mp3: i + "mpeg3",
    mpg: r + "mpeg",
    ogv: r + "ogg",
    xlsx: t + "spreadsheetml.sheet",
    xltx: t + "spreadsheetml.template",
    docx: t + "wordprocessingml.document",
    dotx: t + "wordprocessingml.template",
    pptx: t + "presentationml.presentation",
    potx: t + "presentationml.template",
    ppsx: t + "presentationml.slideshow",
    mdb: e + "vnd.ms-access"
  };
  function c(h, y) {
    h.split(",").forEach((b) => d[b] = y);
  }
  function m(h, y) {
    h.split(",").forEach((b) => d[b] = y(b));
  }
  return m("jpeg,gif,png,tiff,bmp,webp", (h) => l + h), m("jsx,csv,css", (h) => n + h), m("aac,ac3,aiff,m4a,m4b,m4p,mid,midi,wav", (h) => i + h), m("3gpp,avi,dv,divx,ogg,mp4,webm", (h) => r + h), m("rtf,pdf", (h) => e + h), c("htm,html,shtm", n + "html"), c("js,mjs,cjs", n + "javascript"), c("yml,yaml", e + "yaml"), c("bat,cmd", e + "bat"), c("xml,csproj,fsproj,vbproj", n + "xml"), c("txt,ps1", n + "plain"), c("qt,mov", r + "quicktime"), c("doc,dot", e + "msword"), c("xls,xlt,xla", e + "excel"), c("ppt,oit,pps,ppa", e + "vnd.ms-powerpoint"), c("cer,crt,der", e + "x-x509-ca-cert"), c("gz,tgz,zip,rar,lzh,z", e + "x-compressed"), c("aaf,aca,asd,bin,cab,chm,class,cur,db,dat,deploy,dll,dsp,exe,fla,ics,inf,mix,msi,mso,obj,ocx,prm,prx,psd,psp,qxd,sea,snp,so,sqlite,toc,ttf,u32,xmp,xsn,xtp", e + "octet-stream"), d;
})();
let ql = [];
function as(e) {
  return e = e.replace(/"/g, "'"), e = e.replace(/>\s+</g, "><"), e = e.replace(/\s{2,}/g, " "), e.replace(sa, encodeURIComponent);
}
function on(e) {
  return "data:image/svg+xml;utf8," + as(e);
}
function is(e) {
  let t = URL.createObjectURL(e);
  return ql.push(t), t;
}
function rs() {
  ql.forEach((e) => {
    try {
      URL.revokeObjectURL(e);
    } catch (t) {
      console.error("URL.revokeObjectURL", t);
    }
  }), ql = [];
}
function an(e) {
  if (!e)
    return null;
  let t = xl(e, "?");
  return $t(t, "/");
}
function Yt(e) {
  let t = an(e);
  return t == null || t.indexOf(".") === -1 ? null : $t(t, ".").toLowerCase();
}
function rn(e) {
  let t = Yt(e.name);
  return t && ss.indexOf(t) >= 0 ? is(e) : pt(e.name);
}
function un(e) {
  if (!e)
    return !1;
  if (e.startsWith("blob:") || e.startsWith("data:"))
    return !0;
  let t = Yt(e);
  return t && ss.indexOf(t) >= 0 || !1;
}
function pt(e) {
  if (!e)
    return null;
  let t = Yt(e);
  return t == null || un(e) ? e : qt(t) || on(fl.doc);
}
function qt(e) {
  let t = us(e);
  return t && on(t) || null;
}
function us(e) {
  if (fl[e])
    return fl[e];
  for (let t = 0; t < En.length; t++) {
    let l = En[t];
    if (os[l].indexOf(e) >= 0)
      return fl[l];
  }
  return null;
}
function dn(e, t = 2) {
  if (e === 0)
    return "0 Bytes";
  const l = t < 0 ? 0 : t, n = Math.floor(Math.log(e) / Math.log(zn));
  return parseFloat((e / Math.pow(zn, n)).toFixed(l)) + " " + oa[n];
}
function ia(e) {
  return e.files && Array.from(e.files).map((t) => ({ fileName: t.name, contentLength: t.size, filePath: rn(t) }));
}
function Ll(e, t) {
  e.onerror = null, e.src = cn(e.src, t) || "";
}
function cn(e, t) {
  return qt($t(e, ".").toLowerCase()) || (t ? qt(t) || t : null) || qt("doc");
}
function Ql(e) {
  if (!e)
    throw new Error("fileNameOrExt required");
  const t = $t(e, ".").toLowerCase();
  return aa[t] || "application/" + t;
}
function ph() {
  return {
    extSvg: us,
    extSrc: qt,
    getExt: Yt,
    encodeSvg: as,
    canPreview: un,
    getFileName: an,
    getMimeType: Ql,
    formatBytes: dn,
    filePathUri: pt,
    svgToDataUri: on,
    fileImageUri: rn,
    objectUrl: is,
    flush: rs,
    inputFiles: ia,
    iconOnError: Ll,
    iconFallbackSrc: cn
  };
}
class ra {
  constructor(t) {
    _e(this, "view");
    _e(this, "includeTypes");
    Object.assign(this, t);
  }
  getTypeName() {
    return "MetadataApp";
  }
  getMethod() {
    return "GET";
  }
  createResponse() {
    return {};
  }
}
const Dt = "/metadata/app.json", ua = {
  Boolean: "checkbox",
  DateTime: "date",
  DateOnly: "date",
  DateTimeOffset: "date",
  TimeSpan: "time",
  TimeOnly: "time",
  Byte: "number",
  Short: "number",
  Int64: "number",
  Int32: "number",
  UInt16: "number",
  UInt32: "number",
  UInt64: "number",
  Single: "number",
  Double: "number",
  Decimal: "number",
  String: "text",
  Guid: "text",
  Uri: "text"
}, da = {
  number: "Int32",
  checkbox: "Boolean",
  date: "DateTime",
  "datetime-local": "DateTime",
  time: "TimeSpan"
}, Kl = {
  Byte: "byte",
  Int16: "short",
  Int32: "int",
  Int64: "long",
  UInt16: "ushort",
  Unt32: "uint",
  UInt64: "ulong",
  Single: "float",
  Double: "double",
  Decimal: "decimal"
};
[...Object.keys(Kl), ...Object.values(Kl)];
const ca = {
  String: "string",
  Boolean: "bool",
  ...Kl
};
function rl(e) {
  return ca[e] || e;
}
function ds(e, t) {
  return e ? (t || (t = []), e === "Nullable`1" ? rl(t[0]) + "?" : e.endsWith("[]") ? `List<${rl(e.substring(0, e.length - 2))}>` : t.length === 0 ? rl(e) : xl(rl(e), "`") + "<" + t.join(",") + ">") : "";
}
function fa(e) {
  return e && ds(e.name, e.genericArgs);
}
class Ot {
  constructor() {
    _e(this, "Query");
    _e(this, "QueryInto");
    _e(this, "Create");
    _e(this, "Update");
    _e(this, "Patch");
    _e(this, "Delete");
  }
  get AnyQuery() {
    return this.Query || this.QueryInto;
  }
  get AnyUpdate() {
    return this.Patch || this.Update;
  }
  toArray() {
    return [this.Query, this.QueryInto, this.Create, this.Update, this.Patch, this.Delete].filter((l) => !!l).map((l) => l);
  }
  get empty() {
    return !this.Query && !this.QueryInto && !this.Create && !this.Update && !this.Patch && !this.Delete;
  }
  add(t) {
    ze.isQueryInto(t) && !this.QueryInto ? this.QueryInto = t : ze.isQuery(t) && !this.Query ? this.Query = t : ze.isCreate(t) && !this.Create ? this.Create = t : ze.isUpdate(t) && !this.Update ? this.Update = t : ze.isPatch(t) && !this.Patch ? this.Patch = t : ze.isDelete(t) && !this.Delete && (this.Delete = t);
  }
  static from(t) {
    const l = new Ot();
    return t.forEach((n) => {
      l.add(n);
    }), l;
  }
  static forType(t, l) {
    var i;
    let n = new Ot();
    return t && (l ?? (l = (i = ie.metadata.value) == null ? void 0 : i.api), l == null || l.operations.forEach((r) => {
      var d;
      ((d = r.dataModel) == null ? void 0 : d.name) == t && n.add(r);
    })), n;
  }
}
const ze = {
  Create: "ICreateDb`1",
  Update: "IUpdateDb`1",
  Patch: "IPatchDb`1",
  Delete: "IDeleteDb`1",
  AnyRead: ["QueryDb`1", "QueryDb`2"],
  AnyWrite: ["ICreateDb`1", "IUpdateDb`1", "IPatchDb`1", "IDeleteDb`1"],
  isAnyQuery: (e) => Ue(e.request.inherits, (t) => ze.AnyRead.indexOf(t.name) >= 0),
  isQuery: (e) => Ue(e.request.inherits, (t) => t.name === "QueryDb`1"),
  isQueryInto: (e) => Ue(e.request.inherits, (t) => t.name === "QueryDb`2"),
  isCrud: (e) => {
    var t;
    return (t = e.request.implements) == null ? void 0 : t.some((l) => ze.AnyWrite.indexOf(l.name) >= 0);
  },
  isCreate: (e) => ul(e, ze.Create),
  isUpdate: (e) => ul(e, ze.Update),
  isPatch: (e) => ul(e, ze.Patch),
  isDelete: (e) => ul(e, ze.Delete),
  model: (e) => {
    var t, l, n;
    return e ? Ue(e.inherits, (i) => ze.AnyRead.indexOf(i.name) >= 0) ? (t = e.inherits) == null ? void 0 : t.genericArgs[0] : (n = (l = e.implements) == null ? void 0 : l.find((i) => ze.AnyWrite.indexOf(i.name) >= 0)) == null ? void 0 : n.genericArgs[0] : null;
  }
};
function ma(e) {
  var t;
  return ((t = e.input) == null ? void 0 : t.type) || Vl(fn(e));
}
function cs(e) {
  return e.endsWith("?") ? mo(e, 1) : e;
}
function Vl(e) {
  return ua[cs(e)];
}
function va(e) {
  return e && da[e] || "String";
}
function fn(e) {
  return e.type === "Nullable`1" ? e.genericArgs[0] : e.type;
}
function Zl(e) {
  return e && Vl(e) == "number" || !1;
}
function fs(e) {
  return e && e.toLowerCase() == "string" || !1;
}
function ha(e) {
  return e == "List`1" || e.startsWith("List<") || e.endsWith("[]");
}
function ms(e) {
  if (!(e != null && e.type))
    return !1;
  const t = fn(e);
  return e.isValueType && t.indexOf("`") == -1 || e.isEnum ? !1 : Vl(e.type) == null;
}
function vs(e) {
  var l, n, i;
  if (!(e != null && e.type))
    return !1;
  const t = fn(e);
  return e.isValueType && t.indexOf("`") == -1 || e.isEnum || ((l = e.input) == null ? void 0 : l.type) == "file" || ((n = e.input) == null ? void 0 : n.type) == "tag" || ((i = e.input) == null ? void 0 : i.type) == "combobox" ? !0 : Vl(e.type) != null;
}
function Gt(e, t) {
  let l = typeof e == "string" ? Sl(e) : e;
  l || (console.warn(`Metadata not found for: ${e}`), l = { request: { name: e } });
  let n = function() {
    return function(r) {
      Object.assign(this, r);
    };
  }(), i = function() {
    function r(d) {
      Object.assign(this, d);
    }
    return r.prototype.createResponse = function() {
      return l.returnsVoid ? void 0 : new n();
    }, r.prototype.getTypeName = function() {
      return l.request.name;
    }, r.prototype.getMethod = function() {
      return l.method || "POST";
    }, r;
  }();
  return new i(t);
}
function ga(e, t, l = {}) {
  let n = function() {
    return function(r) {
      Object.assign(this, r);
    };
  }(), i = function() {
    function r(d) {
      Object.assign(this, d);
    }
    return r.prototype.createResponse = function() {
      return typeof l.createResponse == "function" ? l.createResponse() : new n();
    }, r.prototype.getTypeName = function() {
      return e;
    }, r.prototype.getMethod = function() {
      return l.method || "POST";
    }, r;
  }();
  return new i(t);
}
function ml(e, t) {
  return e ? (Object.keys(e).forEach((l) => {
    let n = e[l];
    typeof n == "string" && n.startsWith("/Date") && (e[l] = $l(Vt(n)));
  }), e) : {};
}
function pa(e, t) {
  let l = {};
  return Array.from(e.elements).forEach((n) => {
    var y;
    let i = n;
    if (!i.id || i.value == null || i.value === "")
      return;
    const r = i.id.toLowerCase(), d = t && t.find((b) => b.name.toLowerCase() == r);
    let c = d == null ? void 0 : d.type, m = (y = d == null ? void 0 : d.genericArgs) == null ? void 0 : y[0], h = i.type === "checkbox" ? i.checked : i.value;
    Zl(c) ? h = Number(h) : c === "List`1" && typeof h == "string" && (h = h.split(",").map((b) => Zl(m) ? Number(b) : b)), l[i.id] = h;
  }), l;
}
function mn(e) {
  var t;
  return ((t = e == null ? void 0 : e.api) == null ? void 0 : t.operations) && e.api.operations.length > 0;
}
function ya(e) {
  if (e != null && e.assert && !ie.metadata.value)
    throw new Error("useMetadata() not configured, see: https://docs.servicestack.net/vue/use-metadata");
  return ie.metadata.value;
}
function Wt(e) {
  return e && mn(e) ? (e.date = co(new Date()), ie.metadata.value = e, typeof localStorage < "u" && localStorage.setItem(Dt, JSON.stringify(e)), !0) : !1;
}
function ba() {
  ie.metadata.value = null, typeof localStorage < "u" && localStorage.removeItem(Dt);
}
function hs() {
  if (ie.metadata.value != null)
    return !0;
  let e = globalThis.Server;
  if (mn(e))
    Wt(e);
  else {
    const t = typeof localStorage < "u" ? localStorage.getItem(Dt) : null;
    if (t)
      try {
        Wt(JSON.parse(t));
      } catch {
        console.error(`Could not JSON.parse ${Dt} from localStorage`);
      }
  }
  return ie.metadata.value != null;
}
async function Nn(e, t) {
  let l = t ? await t() : await fetch(e);
  if (l.ok) {
    let n = await l.text();
    Wt(JSON.parse(n));
  } else
    console.error(`Could not download ${t ? "AppMetadata" : e}: ${l.statusText}`);
  mn(ie.metadata.value) || console.warn("AppMetadata is not available");
}
async function wa(e) {
  var r;
  const { olderThan: t, resolvePath: l, resolve: n } = e || {};
  let i = hs() && t !== 0;
  if (i && t) {
    let d = Vt((r = ie.metadata.value) == null ? void 0 : r.date);
    (!d || new Date().getTime() - d.getTime() > t) && (i = !1);
  }
  if (!i) {
    if ((l || n) && await Nn(l || Dt, n), ie.metadata.value != null)
      return;
    const d = qe("client");
    if (d != null) {
      const c = await d.api(new ra());
      c.succeeded && Wt(c.response);
    }
    if (ie.metadata.value != null)
      return;
    await Nn(Dt);
  }
  return ie.metadata.value;
}
function st(e, t) {
  var d;
  let l = (d = ie.metadata.value) == null ? void 0 : d.api;
  if (!l || !e)
    return null;
  let n = l.types.find((c) => c.name.toLowerCase() === e.toLowerCase() && (!t || c.namespace == t));
  if (n)
    return n;
  let i = Sl(e);
  if (i)
    return i.request;
  let r = l.operations.find((c) => c.response && c.response.name.toLowerCase() === e.toLowerCase() && (!t || c.response.namespace == t));
  return r ? r.response : null;
}
function Sl(e) {
  var n;
  let t = (n = ie.metadata.value) == null ? void 0 : n.api;
  return t ? t.operations.find((i) => i.request.name.toLowerCase() === e.toLowerCase()) : null;
}
function xa({ dataModel: e }) {
  var n;
  const t = (n = ie.metadata.value) == null ? void 0 : n.api;
  if (!t)
    return [];
  let l = t.operations;
  if (e) {
    const i = typeof e == "string" ? st(e) : e;
    l = l.filter((r) => gs(r.dataModel, i));
  }
  return l;
}
function vn(e) {
  return e ? st(e.name, e.namespace) : null;
}
function gs(e, t) {
  return e && t && e.name === t.name && (!e.namespace || !t.namespace || e.namespace === t.namespace);
}
function ka(e, t) {
  let l = st(e);
  return l && l.properties && l.properties.find((i) => i.name.toLowerCase() === t.toLowerCase());
}
function ps(e) {
  return ys(st(e));
}
function ys(e) {
  if (e && e.isEnum && e.enumNames != null) {
    let t = {};
    for (let l = 0; l < e.enumNames.length; l++) {
      const n = (e.enumDescriptions ? e.enumDescriptions[l] : null) || e.enumNames[l], i = (e.enumValues != null ? e.enumValues[l] : null) || e.enumNames[l];
      t[i] = n;
    }
    return t;
  }
  return null;
}
function bs(e) {
  if (!e)
    return null;
  let t = {}, l = e.input && e.input.allowableEntries;
  if (l) {
    for (let i = 0; i < l.length; i++) {
      let r = l[i];
      t[r.key] = r.value;
    }
    return t;
  }
  let n = e.allowableValues || (e.input ? e.input.allowableValues : null);
  if (n) {
    for (let i = 0; i < n.length; i++) {
      let r = n[i];
      t[r] = r;
    }
    return t;
  }
  if (e.isEnum) {
    const i = e.genericArgs && e.genericArgs.length == 1 ? e.genericArgs[0] : e.type, r = st(i);
    if (r)
      return ys(r);
  }
  return null;
}
function hn(e) {
  if (!e)
    return;
  const t = [];
  return Object.keys(e).forEach((l) => t.push({ key: l, value: e[l] })), t;
}
function $a(e, t) {
  const n = ((i, r) => Object.assign({
    id: i,
    name: i,
    type: r
  }, t))(e.name, (t == null ? void 0 : t.type) || ma(e) || "text");
  return e.isEnum && (n.type = "select", n.allowableEntries = hn(bs(e))), n;
}
function Ca(e) {
  let t = [];
  if (e) {
    const l = Je(e), n = Sl(e.name), i = vn(n == null ? void 0 : n.dataModel);
    l.forEach((r) => {
      var c, m, h;
      if (!vs(r))
        return;
      const d = $a(r, r.input);
      if (d.id = fo(d.id), d.type == "file" && r.uploadTo && !d.accept) {
        const y = (m = (c = ie.metadata.value) == null ? void 0 : c.plugins.filesUpload) == null ? void 0 : m.locations.find((b) => b.name == r.uploadTo);
        y && !d.accept && y.allowExtensions && (d.accept = y.allowExtensions.map((b) => b.startsWith(".") ? b : `.${b}`).join(","));
      }
      if (i) {
        const y = (h = i.properties) == null ? void 0 : h.find((b) => b.name == r.name);
        r.ref || (r.ref = y == null ? void 0 : y.ref);
      }
      if (d.options)
        try {
          const y = {
            input: d,
            $typeFields: l.map((p) => p.name),
            $dataModelFields: i ? Je(i).map((p) => p.name) : [],
            ...ie.config.scopeWhitelist
          }, b = nn(d.options, y);
          Object.keys(b).forEach((p) => {
            d[p] = b[p];
          });
        } catch {
          console.error(`failed to evaluate '${d.options}'`);
        }
      t.push(d);
    });
  }
  return t;
}
function gn(e, t) {
  var i, r;
  if (!t.type)
    return console.error("enumDescriptions missing {type:'EnumType'} options"), [`${e}`];
  const l = st(t.type);
  if (!(l != null && l.enumValues))
    return console.error(`Could not find metadata for ${t.type}`), [`${e}`];
  const n = [];
  for (let d = 0; d < l.enumValues.length; d++) {
    const c = parseInt(l.enumValues[d]);
    c > 0 && (c & e) === c && n.push(((i = l.enumDescriptions) == null ? void 0 : i[d]) || ((r = l.enumNames) == null ? void 0 : r[d]) || `${e}`);
  }
  return n;
}
function ws(e) {
  return (t) => typeof t == "number" ? gn(t, { type: e }) : t;
}
function Je(e) {
  if (!e)
    return [];
  let t = [], l = {};
  function n(i) {
    i.forEach((r) => {
      l[r.name] || (l[r.name] = 1, t.push(r));
    });
  }
  for (; e; )
    e.properties && n(e.properties), e = e.inherits ? vn(e.inherits) : null;
  return t.map((i) => i.type.endsWith("[]") ? { ...i, type: "List`1", genericArgs: [i.type.substring(0, i.type.length - 2)] } : i);
}
function ul(e, t) {
  var l;
  return ((l = e.request.implements) == null ? void 0 : l.some((n) => n.name === t)) || !1;
}
function el(e) {
  return e ? xs(e, Je(e)) : null;
}
function xs(e, t) {
  let l = t.find((r) => r.name.toLowerCase() === "id");
  if (l && l.isPrimaryKey)
    return l;
  let i = t.find((r) => r.isPrimaryKey) || l;
  if (!i) {
    let r = ze.model(e);
    if (r)
      return Ue(st(r), (d) => el(d));
    console.error(`Primary Key not found in ${e.name}`);
  }
  return i || null;
}
function _a(e, t) {
  return Ue(el(e), (l) => be(t, l.name));
}
function ks(e, t, l) {
  return e && e.valueType === "none" ? "" : l.key === "%In" || l.key === "%Between" ? `(${l.value})` : La(t, l.value);
}
function La(e, t) {
  return e ? (e = cs(e), Zl(e) || e === "Boolean" ? t : ha(e) ? `[${t}]` : `'${t}'`) : t;
}
function ot() {
  const e = f(() => {
    var n;
    return ((n = ie.metadata.value) == null ? void 0 : n.app) || null;
  }), t = f(() => {
    var n;
    return ((n = ie.metadata.value) == null ? void 0 : n.api) || null;
  }), l = f(() => {
    var n;
    return ((n = ie.metadata.value) == null ? void 0 : n.plugins.autoQuery.viewerConventions) || [];
  });
  return hs(), {
    loadMetadata: wa,
    getMetadata: ya,
    setMetadata: Wt,
    clearMetadata: ba,
    metadataApp: e,
    metadataApi: t,
    filterDefinitions: l,
    typeOf: st,
    typeOfRef: vn,
    typeEquals: gs,
    apiOf: Sl,
    findApis: xa,
    typeName: fa,
    typeName2: ds,
    property: ka,
    enumOptions: ps,
    propertyOptions: bs,
    createFormLayout: Ca,
    typeProperties: Je,
    supportsProp: vs,
    Crud: ze,
    Apis: Ot,
    getPrimaryKey: el,
    getPrimaryKeyByProps: xs,
    getId: _a,
    createDto: Gt,
    makeDto: ga,
    toFormValues: ml,
    formValues: pa,
    isComplexProp: ms,
    asKvps: hn,
    expandEnumFlags: gn,
    enumFlagsConverter: ws
  };
}
const tt = class {
  static async getOrFetchValue(t, l, n, i, r, d, c) {
    const m = tt.getValue(n, c, r);
    return m ?? (await tt.fetchLookupIds(t, l, n, i, r, d, [c]), tt.getValue(n, c, r));
  }
  static getValue(t, l, n) {
    const i = tt.Lookup[t];
    if (i) {
      const r = i[l];
      if (r)
        return n = n.toLowerCase(), r[n];
    }
  }
  static setValue(t, l, n, i) {
    const r = tt.Lookup[t] ?? (tt.Lookup[t] = {}), d = r[l] ?? (r[l] = {});
    n = n.toLowerCase(), d[n] = i;
  }
  static setRefValue(t, l) {
    const n = be(l, t.refId);
    if (n == null || t.refLabel == null)
      return null;
    const i = be(l, t.refLabel);
    return tt.setValue(t.model, n, t.refLabel, i), i;
  }
  static async fetchLookupIds(t, l, n, i, r, d, c) {
    const m = l.operations.find((h) => {
      var y;
      return ze.isAnyQuery(h) && ((y = h.dataModel) == null ? void 0 : y.name) == n;
    });
    if (m) {
      const h = tt.Lookup[n] ?? (tt.Lookup[n] = {}), y = [];
      Object.keys(h).forEach((U) => {
        const Y = h[U];
        be(Y, r) && y.push(U);
      });
      const b = c.filter((U) => !y.includes(U));
      if (b.length == 0)
        return;
      const p = d ? null : `${i},${r}`, v = {
        [i + "In"]: b.join(",")
      };
      p && (v.fields = p);
      const g = Gt(m, v), O = await t.api(g, { jsconfig: "edv,eccn" });
      if (O.succeeded)
        (be(O.response, "results") || []).forEach((Y) => {
          if (!be(Y, i)) {
            console.error(`result[${i}] == null`, Y);
            return;
          }
          const R = `${be(Y, i)}`, N = be(Y, r);
          r = r.toLowerCase();
          const T = h[R] ?? (h[R] = {});
          T[r] = `${N}`;
        });
      else {
        console.error(`Failed to call ${m.request.name}`);
        return;
      }
    }
  }
};
let wt = tt;
_e(wt, "Lookup", {});
let Gl = () => new Date().getTime(), Va = ["/", "T", ":", "-"], rt = {
  //locale: null,
  assumeUtc: !0,
  //number: null,
  date: {
    method: "Intl.DateTimeFormat",
    options: "{dateStyle:'medium'}"
  },
  maxFieldLength: 150,
  maxNestedFields: 2,
  maxNestedFieldLength: 30
}, Sa = new Intl.RelativeTimeFormat(rt.locale, {}), Un = 24 * 60 * 60 * 1e3 * 365, Bl = {
  year: Un,
  month: Un / 12,
  day: 24 * 60 * 60 * 1e3,
  hour: 60 * 60 * 1e3,
  minute: 60 * 1e3,
  second: 1e3
}, yt = {
  currency: Cs,
  bytes: _s,
  link: Ls,
  linkTel: Vs,
  linkMailTo: Ss,
  icon: Ms,
  iconRounded: As,
  attachment: Ts,
  hidden: Fs,
  time: Is,
  relativeTime: yn,
  relativeTimeFromMs: Ml,
  enumFlags: Os,
  formatDate: Rt,
  formatNumber: pn
};
"iconOnError" in globalThis || (globalThis.iconOnError = Ll);
class Ke {
}
_e(Ke, "currency", { method: "currency" }), _e(Ke, "bytes", { method: "bytes" }), _e(Ke, "link", { method: "link" }), _e(Ke, "linkTel", { method: "linkTel" }), _e(Ke, "linkMailTo", { method: "linkMailTo" }), _e(Ke, "icon", { method: "icon" }), _e(Ke, "iconRounded", { method: "iconRounded" }), _e(Ke, "attachment", { method: "attachment" }), _e(Ke, "time", { method: "time" }), _e(Ke, "relativeTime", { method: "relativeTime" }), _e(Ke, "relativeTimeFromMs", { method: "relativeTimeFromMs" }), _e(Ke, "date", { method: "formatDate" }), _e(Ke, "number", { method: "formatNumber" }), _e(Ke, "hidden", { method: "hidden" }), _e(Ke, "enumFlags", { method: "enumFlags" });
function Ma(e) {
  rt = Object.assign({}, rt, e);
}
function Aa(e) {
  Object.keys(e || {}).forEach((t) => {
    typeof e[t] == "function" && (yt[t] = e[t]);
  });
}
function $s() {
  return yt;
}
function tl(e, t) {
  return t ? it("span", e, t) : e;
}
function Cs(e, t) {
  const l = ut(t, ["currency"]);
  return tl(new Intl.NumberFormat(void 0, { style: "currency", currency: (t == null ? void 0 : t.currency) || "USD" }).format(e), l);
}
function _s(e, t) {
  return tl(dn(e), t);
}
function Ls(e, t) {
  return it("a", e, Cl({ ...t, href: e }));
}
function Vs(e, t) {
  return it("a", e, Cl({ ...t, href: `tel:${e}` }));
}
function Ss(e, t) {
  t || (t = {});
  let { subject: l, body: n } = t, i = ut(t, ["subject", "body"]), r = {};
  return l && (r.subject = l), n && (r.body = n), it("a", e, Cl({ ...i, href: `mailto:${Kt(e, r)}` }));
}
function Ms(e, t) {
  return it("img", void 0, Object.assign({ class: "w-6 h-6", title: e, src: Ft(e), onerror: "iconOnError(this)" }, t));
}
function As(e, t) {
  return it("img", void 0, Object.assign({ class: "w-8 h-8 rounded-full", title: e, src: Ft(e), onerror: "iconOnError(this)" }, t));
}
function Ts(e, t) {
  let l = an(e), i = Yt(l) == null || un(e) ? Ft(e) : cn(e);
  const r = Ft(i);
  let d = t && (t["icon-class"] || t.iconClass), c = it("img", void 0, Object.assign({ class: "w-6 h-6", src: r, onerror: "iconOnError(this,'att')" }, d ? { class: d } : null)), m = `<span class="pl-1">${l}</span>`;
  return it("a", c + m, Object.assign({ class: "flex", href: Ft(e), title: e }, t ? ut(t, ["icon-class", "iconClass"]) : null));
}
function Fs(e) {
  return "";
}
function Is(e, t) {
  let l = typeof e == "string" ? new Date(Gn(e) * 1e3) : kl(e) ? Vt(e) : null;
  return tl(l ? vo(l) : e, t);
}
function Rt(e, t) {
  if (e == null)
    return "";
  let l = typeof e == "number" ? new Date(e) : typeof e == "string" ? Vt(e) : e;
  if (!kl(l))
    return console.warn(`${l} is not a Date value`), e == null ? "" : `${e}`;
  let n = rt.date ? Al(rt.date) : null;
  return tl(typeof n == "function" ? n(l) : Zn(l), t);
}
function pn(e, t) {
  if (typeof e != "number")
    return e;
  let l = rt.number ? Al(rt.number) : null, n = typeof l == "function" ? l(e) : `${e}`;
  return n === "" && (console.warn(`formatNumber(${e}) => ${n}`, l), n = `${e}`), tl(n, t);
}
function Ds(e, t, l) {
  let n = ho(e), i = t ? Al(t) : null;
  if (typeof i == "function") {
    let d = l;
    if (t != null && t.options)
      try {
        d = nn(t.options, l);
      } catch (c) {
        console.error(`Could not evaluate '${t.options}'`, c, ", with scope:", l);
      }
    return i(e, d);
  }
  let r = n != null ? kl(n) ? Rt(n, l) : typeof n == "number" ? pn(n, l) : n : null;
  return r ?? "";
}
function Jt(e, t, l) {
  return _t(e) ? Ds(e, t, l) : Oa(e, t, l);
}
function Ta(e) {
  if (e == null)
    return NaN;
  if (typeof e == "number")
    return e;
  if (kl(e))
    return e.getTime() - Gl();
  if (typeof e == "string") {
    let t = Number(e);
    if (!isNaN(t))
      return t;
    if (e[0] === "P" || e.startsWith("-P"))
      return Gn(e) * 1e3 * -1;
    if (go(e, Va) >= 0)
      return Vt(e).getTime() - Gl();
  }
  return NaN;
}
function Ml(e, t) {
  for (let l in Bl)
    if (Math.abs(e) > Bl[l] || l === "second")
      return (t || Sa).format(Math.round(e / Bl[l]), l);
}
function yn(e, t) {
  let l = Ta(e);
  return isNaN(l) ? "" : Ml(l, t);
}
function Fa(e, t) {
  return Ml(e.getTime() - (t ? t.getTime() : Gl()));
}
function Os(e, t) {
  return gn(e, t).join(", ");
}
function Al(e) {
  if (!e)
    return null;
  let { method: t, options: l } = e, n = `${t}(${l})`, i = yt[n] || yt[t];
  if (typeof i == "function")
    return i;
  let r = e.locale || rt.locale;
  if (t.startsWith("Intl.")) {
    let d = r ? `'${r}'` : "undefined", c = `return new ${t}(${d},${l || "undefined"})`;
    try {
      let m = Function(c)();
      return i = t === "Intl.DateTimeFormat" ? (h) => m.format(Vt(h)) : t === "Intl.NumberFormat" ? (h) => m.format(Number(h)) : t === "Intl.RelativeTimeFormat" ? (h) => yn(h, m) : (h) => m.format(h), yt[n] = i;
    } catch (m) {
      console.error(`Invalid format: ${c}`, m);
    }
  } else {
    let d = globalThis[t];
    if (typeof d == "function") {
      let c = l != null ? Function("return " + l)() : void 0;
      return i = (m) => d(m, c, r), yt[n] = i;
    }
    console.error(`No '${t}' function exists`, Object.keys(yt));
  }
  return null;
}
function js(e, t) {
  return e ? e.length > t ? e.substring(0, t) + "..." : e : "";
}
function Ps(e) {
  return e.substring(0, 6) === "/Date(" ? Rt(Vt(e)) : e;
}
function Ia(e) {
  return bn(jt(e)).replace(/"/g, "");
}
function Bs(e) {
  if (e == null || e === "")
    return "";
  if (typeof e == "string")
    try {
      return JSON.parse(e);
    } catch {
      console.warn("couldn't parse as JSON", e);
    }
  return e;
}
function bn(e, t = 4) {
  return e = Bs(e), typeof e != "object" ? typeof e == "string" ? e : `${e}` : JSON.stringify(e, void 0, t);
}
function Da(e) {
  return e = Bs(e), typeof e != "object" ? typeof e == "string" ? e : `${e}` : (e = Object.assign({}, e), e = jt(e), bn(e));
}
function jt(e) {
  if (e == null)
    return null;
  if (typeof e == "string")
    return Ps(e);
  if (_t(e))
    return e;
  if (e instanceof Date)
    return Rt(e);
  if (Array.isArray(e))
    return e.map(jt);
  if (typeof e == "object") {
    let t = {};
    return Object.keys(e).forEach((l) => {
      l != "__type" && (t[l] = jt(e[l]));
    }), t;
  }
  return e;
}
function Oa(e, t, l) {
  let n = e;
  if (Array.isArray(e)) {
    if (_t(e[0]))
      return n.join(",");
    e[0] != null && (n = e[0]);
  }
  if (n == null)
    return "";
  if (n instanceof Date)
    return Rt(n, l);
  let i = Object.keys(n), r = [];
  for (let d = 0; d < Math.min(rt.maxNestedFields, i.length); d++) {
    let c = i[d], m = `${jt(n[c])}`;
    r.push(`<b class="font-medium">${c}</b>: ${Hl(js(Ps(m), rt.maxNestedFieldLength))}`);
  }
  return i.length > 2 && r.push("..."), it("span", "{ " + r.join(", ") + " }", Object.assign({ title: Hl(Ia(e)) }, l));
}
function yh() {
  return {
    Formats: Ke,
    setDefaultFormats: Ma,
    getFormatters: $s,
    setFormatters: Aa,
    formatValue: Jt,
    formatter: Al,
    dateInputFormat: $l,
    currency: Cs,
    bytes: _s,
    link: Ls,
    linkTel: Vs,
    linkMailTo: Ss,
    icon: Ms,
    iconRounded: As,
    attachment: Ts,
    hidden: Fs,
    time: Is,
    relativeTime: yn,
    relativeTimeFromDate: Fa,
    relativeTimeFromMs: Ml,
    enumFlags: Os,
    formatDate: Rt,
    formatNumber: pn,
    indentJson: bn,
    prettyJson: Da,
    scrub: jt,
    truncate: js,
    apiValueFmt: Ds,
    iconOnError: Ll
  };
}
const ja = ["onClick", "title"], Pa = /* @__PURE__ */ ue({
  __name: "RouterLink",
  props: {
    to: null
  },
  setup(e) {
    const t = e, { config: l } = St(), n = () => l.value.navigate(t.to ?? "/");
    return (i, r) => (a(), u("a", Le({
      onClick: Ne(n, ["prevent"]),
      title: e.to,
      href: "javascript:void(0)"
    }, i.$attrs), [
      Z(i.$slots, "default")
    ], 16, ja));
  }
}), Qt = class {
  static component(t) {
    const l = Qt.components[t];
    if (l)
      return l;
    const n = Rn(t), i = Object.keys(Qt.components).find((r) => Rn(r) === n);
    return i && Qt.components[i] || null;
  }
};
let ie = Qt;
_e(ie, "config", {
  redirectSignIn: "/signin",
  redirectSignOut: "/auth/logout",
  navigate: (t) => location.href = t,
  assetsPathResolver: (t) => t,
  fallbackPathResolver: (t) => t,
  storage: new es(),
  tableIcon: { svg: "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><g fill='none' stroke='currentColor' stroke-width='1.5'><path d='M5 12v6s0 3 7 3s7-3 7-3v-6'/><path d='M5 6v6s0 3 7 3s7-3 7-3V6'/><path d='M12 3c7 0 7 3 7 3s0 3-7 3s-7-3-7-3s0-3 7-3Z'/></g></svg>" },
  scopeWhitelist: {
    enumFlagsConverter: ws,
    ...$s()
  }
}), _e(ie, "autoQueryGridDefaults", {
  deny: [],
  hide: [],
  toolbarButtonClass: void 0,
  tableStyle: "stripedRows",
  take: 25,
  maxFieldLength: 150
}), _e(ie, "events", po()), _e(ie, "user", M(null)), _e(ie, "metadata", M(null)), _e(ie, "components", {
  RouterLink: Pa
});
function Ba(e) {
  ie.config = Object.assign(ie.config, e);
}
function Ra(e) {
  ie.autoQueryGridDefaults = Object.assign(ie.autoQueryGridDefaults, e);
}
function wn(e) {
  return e && ie.config.assetsPathResolver ? ie.config.assetsPathResolver(e) : e;
}
function Ha(e) {
  return e && ie.config.fallbackPathResolver ? ie.config.fallbackPathResolver(e) : e;
}
function St() {
  const e = f(() => ie.config), t = f(() => ie.autoQueryGridDefaults), l = ie.events;
  return {
    config: e,
    setConfig: Ba,
    events: l,
    autoQueryGridDefaults: t,
    setAutoQueryGridDefaults: Ra,
    assetsPathResolver: wn,
    fallbackPathResolver: Ha
  };
}
const Rs = ue({
  inheritAttrs: !1,
  props: {
    image: Object,
    svg: String,
    src: String,
    alt: String,
    type: String
  },
  setup(e, { attrs: t }) {
    return () => {
      let l = e.image;
      if (e.type) {
        const { typeOf: r } = ot(), d = r(e.type);
        d || console.warn(`Type ${e.type} does not exist`), d != null && d.icon ? l = d == null ? void 0 : d.icon : console.warn(`Type ${e.type} does not have a [Svg] icon`);
      }
      let n = e.svg || (l == null ? void 0 : l.svg) || "";
      if (n.startsWith("<svg ")) {
        let d = xl(n, ">").indexOf("class="), c = `${(l == null ? void 0 : l.cls) || ""} ${t.class || ""}`;
        if (d == -1)
          n = `<svg class="${c}" ${n.substring(4)}`;
        else {
          const m = d + 6 + 1;
          n = `${n.substring(0, m) + c} ${n.substring(m)}`;
        }
        return ht("span", { innerHTML: n });
      } else
        return ht("img", {
          class: [l == null ? void 0 : l.cls, t.class],
          src: wn(e.src || (l == null ? void 0 : l.uri)),
          onError: (r) => Ll(r.target)
        });
    };
  }
}), Ea = { class: "text-2xl font-semibold text-gray-900 dark:text-gray-300" }, za = { class: "flex" }, Na = /* @__PURE__ */ s("path", {
  d: "M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z",
  fill: "currentColor"
}, null, -1), Ua = /* @__PURE__ */ s("path", {
  d: "M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z",
  fill: "currentFill"
}, null, -1), qa = [
  Na,
  Ua
], Qa = /* @__PURE__ */ ue({
  __name: "Loading",
  props: {
    imageClass: { default: "w-6 h-6" }
  },
  setup(e) {
    return (t, l) => (a(), u("div", Ea, [
      s("div", za, [
        (a(), u("svg", {
          class: w(["self-center inline mr-2 text-gray-200 animate-spin dark:text-gray-600 fill-gray-600 dark:fill-gray-300", e.imageClass]),
          role: "status",
          viewBox: "0 0 100 101",
          fill: "none",
          xmlns: "http://www.w3.org/2000/svg"
        }, qa, 2)),
        s("span", null, [
          Z(t.$slots, "default")
        ])
      ])
    ]));
  }
}), Ka = ["href", "onClick"], Za = ["type"], Ga = /* @__PURE__ */ ue({
  __name: "OutlineButton",
  props: {
    type: { default: "submit" },
    href: null
  },
  setup(e) {
    const t = "inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 shadow-sm text-sm font-medium rounded-md text-gray-700 dark:text-gray-200 disabled:text-gray-400 bg-white dark:bg-black hover:bg-gray-50 hover:dark:bg-gray-900 disabled:hover:bg-white dark:disabled:hover:bg-black focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black";
    return (l, n) => {
      const i = X("router-link");
      return e.href ? (a(), se(i, {
        key: 0,
        to: e.href
      }, {
        default: ke(({ navigate: r }) => [
          s("button", {
            class: w(t),
            href: e.href,
            onClick: r
          }, [
            Z(l.$slots, "default")
          ], 8, Ka)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), u("button", Le({
        key: 1,
        type: e.type,
        class: t
      }, l.$attrs), [
        Z(l.$slots, "default")
      ], 16, Za));
    };
  }
}), Wa = ["href", "onClick"], Ja = ["type"], Xa = /* @__PURE__ */ ue({
  __name: "PrimaryButton",
  props: {
    type: { default: "submit" },
    href: null,
    color: { default: "indigo" }
  },
  setup(e) {
    const t = e, l = {
      blue: "text-white bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 disabled:hover:bg-blue-400 focus:ring-indigo-500 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800",
      purple: "text-white bg-purple-600 hover:bg-purple-700 disabled:bg-purple-400 disabled:hover:bg-purple-400 focus:ring-indigo-500 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800",
      red: "focus:ring-red-500 text-white bg-red-600 hover:bg-red-700 disabled:bg-red-400 disabled:hover:bg-red-400 focus:ring-red-500 dark:bg-red-600 dark:hover:bg-red-700 dark:focus:ring-red-500",
      green: "focus:ring-green-300 text-white bg-green-600 hover:bg-green-700 disabled:bg-green-400 disabled:hover:bg-green-400 focus:ring-green-500 dark:bg-green-600 dark:hover:bg-green-700 dark:focus:ring-green-500",
      sky: "focus:ring-sky-300 text-white bg-sky-600 hover:bg-sky-700 disabled:bg-sky-400 disabled:hover:bg-sky-400 focus:ring-sky-500 dark:bg-sky-600 dark:hover:bg-sky-700 dark:focus:ring-sky-500",
      cyan: "focus:ring-cyan-300 text-white bg-cyan-600 hover:bg-cyan-700 disabled:bg-cyan-400 disabled:hover:bg-cyan-400 focus:ring-cyan-500 dark:bg-cyan-600 dark:hover:bg-cyan-700 dark:focus:ring-cyan-500",
      indigo: "focus:ring-2 focus:ring-offset-2 text-white bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 disabled:hover:bg-indigo-400 focus:ring-indigo-500 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
    }, n = f(() => "inline-flex justify-center rounded-md border border-transparent py-2 px-4 text-sm font-medium shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 dark:ring-offset-black " + (l[t.color] || l.indigo));
    return (i, r) => {
      const d = X("router-link");
      return e.href ? (a(), se(d, {
        key: 0,
        to: e.href
      }, {
        default: ke(({ navigate: c }) => [
          s("button", {
            class: w(o(n)),
            href: e.href,
            onClick: c
          }, [
            Z(i.$slots, "default")
          ], 10, Wa)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), u("button", Le({
        key: 1,
        type: e.type,
        class: o(n)
      }, i.$attrs), [
        Z(i.$slots, "default")
      ], 16, Ja));
    };
  }
}), Ya = ["type", "href", "onClick"], ei = ["type"], ti = /* @__PURE__ */ ue({
  __name: "SecondaryButton",
  props: {
    type: null,
    href: null
  },
  setup(e) {
    const t = "inline-flex justify-center rounded-md border border-gray-300 py-2 px-4 text-sm font-medium shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-400 dark:hover:text-white hover:bg-gray-50 dark:hover:bg-gray-700 focus:ring-indigo-500 dark:focus:ring-indigo-600 dark:ring-offset-black";
    return (l, n) => {
      const i = X("router-link");
      return e.href ? (a(), se(i, {
        key: 0,
        to: e.href
      }, {
        default: ke(({ navigate: r }) => [
          s("button", {
            type: e.type ?? "button",
            class: w(t),
            href: e.href,
            onClick: r
          }, [
            Z(l.$slots, "default")
          ], 8, Ya)
        ]),
        _: 3
      }, 8, ["to"])) : (a(), u("button", Le({
        key: 1,
        type: e.type ?? "button",
        class: t
      }, l.$attrs), [
        Z(l.$slots, "default")
      ], 16, ei));
    };
  }
});
function Ge(e, t) {
  return Array.isArray(e) ? e.indexOf(t) >= 0 : e == t || e.includes(t);
}
const bl = {
  blue: "text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-200",
  purple: "text-purple-600 dark:text-purple-400 hover:text-purple-800 dark:hover:text-purple-200",
  red: "text-red-700 dark:text-red-400 hover:text-red-900 dark:hover:text-red-200",
  green: "text-green-600 dark:text-green-400 hover:text-green-800 dark:hover:text-green-200",
  sky: "text-sky-600 dark:text-sky-400 hover:text-sky-800 dark:hover:text-sky-200",
  cyan: "text-cyan-600 dark:text-cyan-400 hover:text-cyan-800 dark:hover:text-cyan-200",
  indigo: "text-indigo-600 dark:text-indigo-400 hover:text-indigo-800 dark:hover:text-indigo-200"
}, lt = {
  base: "block w-full sm:text-sm rounded-md dark:text-white dark:bg-gray-900 disabled:bg-slate-50 disabled:text-slate-500 disabled:border-slate-200 disabled:shadow-none",
  invalid: "pr-10 border-red-300 text-red-900 placeholder-red-300 focus:outline-none focus:ring-red-500 focus:border-red-500",
  valid: "shadow-sm focus:ring-indigo-500 focus:border-indigo-500 border-gray-300 dark:border-gray-600"
}, Ut = {
  panelClass: "shadow sm:rounded-md",
  formClass: "space-y-6 bg-white dark:bg-black py-6 px-4 sm:p-6",
  headingClass: "text-lg font-medium leading-6 text-gray-900 dark:text-gray-100",
  subHeadingClass: "mt-1 text-sm text-gray-500 dark:text-gray-400"
}, Tt = {
  panelClass: "pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg",
  formClass: "flex h-full flex-col divide-y divide-gray-200 dark:divide-gray-700 shadow-xl bg-white dark:bg-black",
  titlebarClass: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6",
  headingClass: "text-lg font-medium text-gray-900 dark:text-gray-100",
  subHeadingClass: "mt-1 text-sm text-gray-500 dark:text-gray-400",
  closeButtonClass: "rounded-md bg-gray-50 dark:bg-gray-900 text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:ring-offset-black"
}, Wl = {
  modalClass: "relative transform overflow-hidden rounded-lg bg-white dark:bg-black text-left shadow-xl transition-all sm:my-8",
  sizeClass: "sm:max-w-prose lg:max-w-screen-md xl:max-w-screen-lg 2xl:max-w-screen-xl sm:w-full"
}, Ze = {
  panelClass(e = "slideOver") {
    return e == "card" ? Ut.panelClass : Tt.panelClass;
  },
  formClass(e = "slideOver") {
    return e == "card" ? Ut.formClass : Tt.formClass;
  },
  headingClass(e = "slideOver") {
    return e == "card" ? Ut.headingClass : Tt.headingClass;
  },
  subHeadingClass(e = "slideOver") {
    return e == "card" ? Ut.subHeadingClass : Tt.subHeadingClass;
  },
  buttonsClass: "mt-4 px-4 py-3 bg-gray-50 dark:bg-gray-900 sm:px-6 flex flex-wrap justify-between",
  legendClass: "text-base font-medium text-gray-900 dark:text-gray-100 text-center mb-4"
}, he = {
  getGridClass(e = "stripedRows") {
    return he.gridClass;
  },
  getGrid2Class(e = "stripedRows") {
    return Ge(e, "fullWidth") ? "overflow-x-auto" : he.grid2Class;
  },
  getGrid3Class(e = "stripedRows") {
    return Ge(e, "fullWidth") ? "inline-block min-w-full py-2 align-middle" : he.grid3Class;
  },
  getGrid4Class(e = "stripedRows") {
    return Ge(e, "whiteBackground") ? "" : Ge(e, "fullWidth") ? "overflow-hidden shadow-sm ring-1 ring-black ring-opacity-5" : he.grid4Class;
  },
  getTableClass(e = "stripedRows") {
    return Ge(e, "fullWidth") || Ge(e, "verticalLines") ? "min-w-full divide-y divide-gray-300" : he.tableClass;
  },
  getTheadClass(e = "stripedRows") {
    return Ge(e, "whiteBackground") ? "" : he.theadClass;
  },
  getTheadRowClass(e = "stripedRows") {
    return he.theadRowClass + (Ge(e, "verticalLines") ? " divide-x divide-gray-200 dark:divide-gray-700" : "");
  },
  getTheadCellClass(e = "stripedRows") {
    return he.theadCellClass + (Ge(e, "uppercaseHeadings") ? " uppercase" : "");
  },
  getTbodyClass(e = "stripedRows") {
    return (Ge(e, "whiteBackground") || Ge(e, "verticalLines") ? "divide-y divide-gray-200 dark:divide-gray-800" : he.tableClass) + (Ge(e, "verticalLines") ? " bg-white" : "");
  },
  getTableRowClass(e = "stripedRows", t, l, n) {
    return (n ? "cursor-pointer " : "") + (l ? "bg-indigo-100 dark:bg-blue-800" : (n ? "hover:bg-yellow-50 dark:hover:bg-blue-900 " : "") + (Ge(e, "stripedRows") ? t % 2 == 0 ? "bg-white dark:bg-black" : "bg-gray-50 dark:bg-gray-800" : "bg-white dark:bg-black")) + (Ge(e, "verticalLines") ? " divide-x divide-gray-200 dark:divide-gray-700" : "");
  },
  gridClass: "flex flex-col",
  //original -margins + padding forces scroll bars when parent is w-full for no clear benefits?
  //original: -my-2 -mx-4 overflow-x-auto sm:-mx-6 lg:-mx-8
  grid2Class: "",
  //original: inline-block min-w-full py-2 align-middle md:px-6 lg:px-8
  grid3Class: "inline-block min-w-full py-2 align-middle",
  grid4Class: "overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg",
  tableClass: "min-w-full divide-y divide-gray-200 dark:divide-gray-700",
  theadClass: "bg-gray-50 dark:bg-gray-900",
  tableCellClass: "px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400",
  theadRowClass: "select-none",
  theadCellClass: "px-6 py-4 text-left text-sm font-medium tracking-wider whitespace-nowrap",
  toolbarButtonClass: "inline-flex items-center px-2.5 py-1.5 border border-gray-300 dark:border-gray-700 shadow-sm text-sm font-medium rounded text-gray-700 dark:text-gray-300 bg-white dark:bg-black hover:bg-gray-50 dark:hover:bg-gray-900 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black"
}, li = {
  colspans: "col-span-3 sm:col-span-3"
}, bh = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  a: bl,
  card: Ut,
  dummy: li,
  form: Ze,
  grid: he,
  input: lt,
  modal: Wl,
  slideOver: Tt
}, Symbol.toStringTag, { value: "Module" })), ni = /* @__PURE__ */ ue({
  __name: "TextLink",
  props: {
    color: { default: "blue" }
  },
  setup(e) {
    const t = e, l = to(), n = f(() => (bl[t.color] || bl.blue) + (l.href ? "" : " cursor-pointer"));
    return (i, r) => (a(), u("a", {
      class: w(o(n))
    }, [
      Z(i.$slots, "default")
    ], 2));
  }
}), si = {
  class: "flex",
  "aria-label": "Breadcrumb"
}, oi = {
  role: "list",
  class: "flex items-center space-x-4"
}, ai = ["href", "title"], ii = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6 flex-shrink-0",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M9.293 2.293a1 1 0 011.414 0l7 7A1 1 0 0117 11h-1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-3a1 1 0 00-1-1H9a1 1 0 00-1 1v3a1 1 0 01-1 1H5a1 1 0 01-1-1v-6H3a1 1 0 01-.707-1.707l7-7z",
    "clip-rule": "evenodd"
  })
], -1), ri = { class: "sr-only" }, ui = /* @__PURE__ */ ue({
  __name: "Breadcrumbs",
  props: {
    homeHref: { default: "/" },
    homeLabel: { default: "Home" }
  },
  setup(e) {
    return (t, l) => (a(), u("nav", si, [
      s("ol", oi, [
        s("li", null, [
          s("div", null, [
            s("a", {
              href: e.homeHref,
              class: "text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400",
              title: e.homeLabel
            }, [
              ii,
              s("span", ri, A(e.homeLabel), 1)
            ], 8, ai)
          ])
        ]),
        Z(t.$slots, "default")
      ])
    ]));
  }
}), di = { class: "flex items-center" }, ci = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6 flex-shrink-0 text-gray-400 dark:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z",
    "clip-rule": "evenodd"
  })
], -1), fi = ["href", "title"], mi = ["title"], vi = /* @__PURE__ */ ue({
  __name: "Breadcrumb",
  props: {
    href: null,
    title: null
  },
  setup(e) {
    return (t, l) => (a(), u("li", null, [
      s("div", di, [
        ci,
        e.href ? (a(), u("a", {
          key: 0,
          href: e.href,
          class: "ml-4 text-lg font-medium text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300",
          title: e.title
        }, [
          Z(t.$slots, "default")
        ], 8, fi)) : (a(), u("span", {
          key: 1,
          class: "ml-4 text-lg font-medium text-gray-700 dark:text-gray-300",
          title: e.title
        }, [
          Z(t.$slots, "default")
        ], 8, mi))
      ])
    ]));
  }
}), hi = {
  key: 0,
  class: "text-base font-semibold text-gray-500 dark:text-gray-400"
}, gi = {
  role: "list",
  class: "mt-4 divide-y divide-gray-200 dark:divide-gray-800 border-t border-b border-gray-200 dark:border-gray-800"
}, pi = /* @__PURE__ */ ue({
  __name: "NavList",
  props: {
    title: null
  },
  setup(e) {
    return (t, l) => (a(), u("div", null, [
      e.title ? (a(), u("h2", hi, A(e.title), 1)) : k("", !0),
      s("ul", gi, [
        Z(t.$slots, "default")
      ])
    ]));
  }
}), yi = { class: "relative flex items-start space-x-4 py-6" }, bi = { class: "flex-shrink-0" }, wi = { class: "flex h-12 w-12 items-center justify-center rounded-lg bg-indigo-50 dark:bg-indigo-900" }, xi = { class: "min-w-0 flex-1" }, ki = { class: "text-base font-medium text-gray-900 dark:text-gray-100" }, $i = { class: "rounded-sm focus-within:ring-2 focus-within:ring-indigo-500 focus-within:ring-offset-2" }, Ci = ["href"], _i = /* @__PURE__ */ s("span", {
  class: "absolute inset-0",
  "aria-hidden": "true"
}, null, -1), Li = { class: "text-base text-gray-500" }, Vi = /* @__PURE__ */ s("div", { class: "flex-shrink-0 self-center" }, [
  /* @__PURE__ */ s("svg", {
    class: "h-5 w-5 text-gray-400",
    xmlns: "http://www.w3.org/2000/svg",
    viewBox: "0 0 20 20",
    fill: "currentColor",
    "aria-hidden": "true"
  }, [
    /* @__PURE__ */ s("path", {
      "fill-rule": "evenodd",
      d: "M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z",
      "clip-rule": "evenodd"
    })
  ])
], -1), Si = /* @__PURE__ */ ue({
  __name: "NavListItem",
  props: {
    title: null,
    href: null,
    icon: null,
    iconSvg: null,
    iconSrc: null,
    iconAlt: null
  },
  setup(e) {
    return (t, l) => {
      const n = X("Icon");
      return a(), u("li", yi, [
        s("div", bi, [
          s("span", wi, [
            $e(n, {
              class: "w-6 h-6 text-indigo-700 dark:text-indigo-300",
              image: e.icon,
              src: e.iconSrc,
              svg: e.iconSvg,
              alt: e.iconAlt
            }, null, 8, ["image", "src", "svg", "alt"])
          ])
        ]),
        s("div", xi, [
          s("h3", ki, [
            s("span", $i, [
              s("a", {
                href: e.href,
                class: "focus:outline-none"
              }, [
                _i,
                xe(" " + A(e.title), 1)
              ], 8, Ci)
            ])
          ]),
          s("p", Li, [
            Z(t.$slots, "default")
          ])
        ]),
        Vi
      ]);
    };
  }
});
function Mi(e) {
  ie.user.value = e, ie.events.publish("signIn", e);
}
function Ai() {
  ie.user.value = null, ie.events.publish("signOut", null);
}
function Hs(e) {
  var t;
  return (((t = ie.user.value) == null ? void 0 : t.roles) || []).indexOf(e) >= 0;
}
function Ti(e) {
  var t;
  return (((t = ie.user.value) == null ? void 0 : t.permissions) || []).indexOf(e) >= 0;
}
function xn() {
  return Hs("Admin");
}
function vl(e) {
  if (!e)
    return !1;
  if (!e.requiresAuth)
    return !0;
  const t = ie.user.value;
  if (!t)
    return !1;
  if (xn())
    return !0;
  let [l, n] = [t.roles || [], t.permissions || []], [i, r, d, c] = [
    e.requiredRoles || [],
    e.requiredPermissions || [],
    e.requiresAnyRole || [],
    e.requiresAnyPermission || []
  ];
  return !(!i.every((m) => l.indexOf(m) >= 0) || d.length > 0 && !d.some((m) => l.indexOf(m) >= 0) || !r.every((m) => n.indexOf(m) >= 0) || c.length > 0 && !c.every((m) => n.indexOf(m) >= 0));
}
function Fi(e) {
  if (!e || !e.requiresAuth)
    return null;
  const t = ie.user.value;
  if (!t)
    return `<b>${e.request.name}</b> requires Authentication`;
  if (xn())
    return null;
  let [l, n] = [t.roles || [], t.permissions || []], [i, r, d, c] = [
    e.requiredRoles || [],
    e.requiredPermissions || [],
    e.requiresAnyRole || [],
    e.requiresAnyPermission || []
  ], m = i.filter((y) => l.indexOf(y) < 0);
  if (m.length > 0)
    return `Requires ${m.map((y) => "<b>" + y + "</b>").join(", ")} Role` + (m.length > 1 ? "s" : "");
  let h = r.filter((y) => n.indexOf(y) < 0);
  return h.length > 0 ? `Requires ${h.map((y) => "<b>" + y + "</b>").join(", ")} Permission` + (h.length > 1 ? "s" : "") : d.length > 0 && !d.some((y) => l.indexOf(y) >= 0) ? `Requires any ${d.filter((y) => l.indexOf(y) < 0).map((y) => "<b>" + y + "</b>").join(", ")} Role` + (m.length > 1 ? "s" : "") : c.length > 0 && !c.every((y) => n.indexOf(y) >= 0) ? `Requires any ${c.filter((y) => n.indexOf(y) < 0).map((y) => "<b>" + y + "</b>").join(", ")} Permission` + (h.length > 1 ? "s" : "") : null;
}
function kn() {
  const e = f(() => ie.user.value || null), t = f(() => ie.user.value != null);
  return { signIn: Mi, signOut: Ai, user: e, isAuthenticated: t, hasRole: Hs, hasPermission: Ti, isAdmin: xn, canAccess: vl, invalidAccessMessage: Fi };
}
const Ii = { key: 0 }, Di = { class: "md:p-4" }, Es = /* @__PURE__ */ ue({
  __name: "EnsureAccess",
  props: {
    invalidAccess: null,
    alertClass: null
  },
  emits: ["done"],
  setup(e) {
    const { isAuthenticated: t } = kn(), { config: l } = St(), n = () => {
      let r = location.href.substring(location.origin.length) || "/";
      const d = Kt(l.value.redirectSignIn, { redirect: r });
      l.value.navigate(d);
    }, i = () => {
      let r = location.href.substring(location.origin.length) || "/";
      const d = Kt(l.value.redirectSignOut, { ReturnUrl: r });
      l.value.navigate(d);
    };
    return (r, d) => {
      const c = X("Alert"), m = X("SecondaryButton");
      return e.invalidAccess ? (a(), u("div", Ii, [
        $e(c, {
          class: w(e.alertClass),
          innerHTML: e.invalidAccess
        }, null, 8, ["class", "innerHTML"]),
        s("div", Di, [
          o(t) ? (a(), se(m, {
            key: 1,
            onClick: i
          }, {
            default: ke(() => [
              xe("Sign Out")
            ]),
            _: 1
          })) : (a(), se(m, {
            key: 0,
            onClick: n
          }, {
            default: ke(() => [
              xe("Sign In")
            ]),
            _: 1
          }))
        ])
      ])) : k("", !0);
    };
  }
}), Oi = { class: "absolute top-0 right-0 bg-white dark:bg-black border dark:border-gray-800 rounded normal-case text-sm shadow w-80" }, ji = { class: "p-4" }, Pi = /* @__PURE__ */ s("h3", { class: "text-base font-medium mb-3 dark:text-gray-100" }, "Sort", -1), Bi = { class: "flex w-full justify-center" }, Ri = /* @__PURE__ */ s("svg", {
  class: "w-6 h-6",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 16 16"
}, [
  /* @__PURE__ */ s("g", { fill: "currentColor" }, [
    /* @__PURE__ */ s("path", {
      "fill-rule": "evenodd",
      d: "M10.082 5.629L9.664 7H8.598l1.789-5.332h1.234L13.402 7h-1.12l-.419-1.371h-1.781zm1.57-.785L11 2.687h-.047l-.652 2.157h1.351z"
    }),
    /* @__PURE__ */ s("path", { d: "M12.96 14H9.028v-.691l2.579-3.72v-.054H9.098v-.867h3.785v.691l-2.567 3.72v.054h2.645V14zm-8.46-.5a.5.5 0 0 1-1 0V3.707L2.354 4.854a.5.5 0 1 1-.708-.708l2-1.999l.007-.007a.498.498 0 0 1 .7.006l2 2a.5.5 0 1 1-.707.708L4.5 3.707V13.5z" })
  ])
], -1), Hi = /* @__PURE__ */ s("span", null, "ASC", -1), Ei = [
  Ri,
  Hi
], zi = /* @__PURE__ */ wl('<svg class="w-6 h-6" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16"><g fill="currentColor"><path d="M12.96 7H9.028v-.691l2.579-3.72v-.054H9.098v-.867h3.785v.691l-2.567 3.72v.054h2.645V7z"></path><path fill-rule="evenodd" d="M10.082 12.629L9.664 14H8.598l1.789-5.332h1.234L13.402 14h-1.12l-.419-1.371h-1.781zm1.57-.785L11 9.688h-.047l-.652 2.156h1.351z"></path><path d="M4.5 2.5a.5.5 0 0 0-1 0v9.793l-1.146-1.147a.5.5 0 0 0-.708.708l2 1.999l.007.007a.497.497 0 0 0 .7-.006l2-2a.5.5 0 0 0-.707-.708L4.5 12.293V2.5z"></path></g></svg><span>DESC</span>', 2), Ni = [
  zi
], Ui = /* @__PURE__ */ s("h3", { class: "text-base font-medium mt-4 mb-2" }, " Filter ", -1), qi = { key: 0 }, Qi = ["id", "value"], Ki = ["for"], Zi = { key: 1 }, Gi = { class: "mb-2" }, Wi = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 text-indigo-700" }, Ji = ["onClick"], Xi = /* @__PURE__ */ s("svg", {
  class: "h-2 w-2",
  stroke: "currentColor",
  fill: "none",
  viewBox: "0 0 8 8"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-width": "1.5",
    d: "M1 1l6 6m0-6L1 7"
  })
], -1), Yi = [
  Xi
], er = { class: "flex" }, tr = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
    "clip-rule": "evenodd"
  })
], -1), lr = [
  tr
], nr = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse" }, $n = /* @__PURE__ */ ue({
  __name: "FilterColumn",
  props: {
    definitions: null,
    column: null,
    topLeft: null
  },
  emits: ["done", "save"],
  setup(e, { emit: t }) {
    const l = e, n = M(), i = M(""), r = M(""), d = M([]), c = f(() => l.column.meta.isEnum == !0), m = f(() => st(l.column.meta.type === "Nullable`1" ? l.column.meta.genericArgs[0] : l.column.meta.type)), h = f(() => l.column.meta.isEnum == !0 ? hn(ps(m.value.name)) : []), y = f(() => {
      var j;
      return ((j = v(l.column.type)) == null ? void 0 : j.map((L) => ({ key: L.value, value: L.name }))) || [];
    }), b = M({ filters: [] }), p = f(() => b.value.filters);
    hl(() => b.value = Object.assign({}, l.column.settings, {
      filters: Array.from(l.column.settings.filters)
    })), hl(() => {
      var L, q, D, K, ne;
      let j = ((D = (q = (L = l.column.settings.filters) == null ? void 0 : L[0]) == null ? void 0 : q.value) == null ? void 0 : D.split(",")) || [];
      if (j.length > 0 && ((K = m.value) != null && K.isEnumInt)) {
        const ee = parseInt(j[0]);
        j = ((ne = m.value.enumValues) == null ? void 0 : ne.filter((te) => (ee & parseInt(te)) > 0)) || [];
      }
      d.value = j;
    });
    function v(j) {
      let L = l.definitions;
      return fs(j) || (L = L.filter((q) => q.types !== "string")), L;
    }
    function g(j, L) {
      return v(j).find((q) => q.value === L);
    }
    function O() {
      var L;
      if (!i.value)
        return;
      let j = (L = g(l.column.type, i.value)) == null ? void 0 : L.name;
      j && (b.value.filters.push({ key: i.value, name: j, value: r.value }), i.value = r.value = "");
    }
    function U(j) {
      b.value.filters.splice(j, 1);
    }
    function Y(j) {
      return ks(g(l.column.type, j.key), l.column.type, j);
    }
    function R() {
      t("done");
    }
    function N() {
      var j;
      i.value = "%", (j = n.value) == null || j.focus();
    }
    function T() {
      var j;
      if (r.value && O(), c.value) {
        let L = Object.values(d.value).filter((q) => q);
        b.value.filters = L.length > 0 ? (j = m.value) != null && j.isEnumInt ? [{ key: "%HasAny", name: "HasAny", value: L.map((q) => parseInt(q)).reduce((q, D) => q + D, 0).toString() }] : [{ key: "%In", name: "In", value: L.join(",") }] : [];
      }
      t("save", b.value), t("done");
    }
    function J(j) {
      b.value.sort = j === b.value.sort ? void 0 : j, xt(T);
    }
    return (j, L) => {
      var ee;
      const q = X("SelectInput"), D = X("TextInput"), K = X("PrimaryButton"), ne = X("SecondaryButton");
      return a(), u("div", {
        class: "fixed z-20 inset-0 overflow-y-auto",
        onClick: R,
        onVnodeMounted: N
      }, [
        s("div", {
          class: "absolute",
          style: Jl(`top:${e.topLeft.y}px;left:${e.topLeft.x}px`),
          onClick: L[5] || (L[5] = Ne(() => {
          }, ["stop"]))
        }, [
          s("div", Oi, [
            s("div", ji, [
              Pi,
              s("div", Bi, [
                s("button", {
                  type: "button",
                  title: "Sort Ascending",
                  onClick: L[0] || (L[0] = (te) => J("ASC")),
                  class: w(`${b.value.sort === "ASC" ? "bg-indigo-100 border-indigo-500" : "bg-white hover:bg-gray-50 border-gray-300"} mr-1 inline-flex items-center px-2.5 py-1.5 border shadow-sm text-sm font-medium rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500`)
                }, Ei, 2),
                s("button", {
                  type: "button",
                  title: "Sort Descending",
                  onClick: L[1] || (L[1] = (te) => J("DESC")),
                  class: w(`${b.value.sort === "DESC" ? "bg-indigo-100 border-indigo-500" : "bg-white hover:bg-gray-50 border-gray-300"} ml-1 inline-flex items-center px-2.5 py-1.5 border shadow-sm text-sm font-medium rounded text-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500`)
                }, Ni, 2)
              ]),
              Ui,
              o(c) ? (a(), u("div", qi, [
                (a(!0), u(Te, null, Ie(o(h), (te) => (a(), u("div", {
                  key: te.key,
                  class: "flex items-center"
                }, [
                  kt(s("input", {
                    type: "checkbox",
                    id: te.key,
                    value: te.key,
                    "onUpdate:modelValue": L[2] || (L[2] = (S) => d.value = S),
                    class: "h-4 w-4 border-gray-300 rounded text-indigo-600 focus:ring-indigo-500"
                  }, null, 8, Qi), [
                    [Xl, d.value]
                  ]),
                  s("label", {
                    for: te.key,
                    class: "ml-3"
                  }, A(te.value), 9, Ki)
                ]))), 128))
              ])) : (a(), u("div", Zi, [
                (a(!0), u(Te, null, Ie(o(p), (te, S) => (a(), u("div", Gi, [
                  s("span", Wi, [
                    xe(A(e.column.name) + " " + A(te.name) + " " + A(Y(te)) + " ", 1),
                    s("button", {
                      type: "button",
                      onClick: (le) => U(S),
                      class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 hover:bg-indigo-200 hover:text-indigo-500 focus:outline-none focus:bg-indigo-500 focus:text-white"
                    }, Yi, 8, Ji)
                  ])
                ]))), 256)),
                s("div", er, [
                  $e(q, {
                    id: "filterRule",
                    class: "w-32 mr-1",
                    modelValue: i.value,
                    "onUpdate:modelValue": L[3] || (L[3] = (te) => i.value = te),
                    entries: o(y),
                    label: "",
                    placeholder: ""
                  }, null, 8, ["modelValue", "entries"]),
                  ((ee = g(e.column.type, i.value)) == null ? void 0 : ee.valueType) !== "none" ? (a(), se(D, {
                    key: 0,
                    ref_key: "txtFilter",
                    ref: n,
                    id: "filterValue",
                    class: "w-32 mr-1",
                    type: "text",
                    modelValue: r.value,
                    "onUpdate:modelValue": L[4] || (L[4] = (te) => r.value = te),
                    onKeyup: Qn(O, ["enter"]),
                    label: "",
                    placeholder: ""
                  }, null, 8, ["modelValue", "onKeyup"])) : k("", !0),
                  s("div", { class: "pt-1" }, [
                    s("button", {
                      type: "button",
                      onClick: O,
                      class: "inline-flex items-center p-1 border border-transparent rounded-full shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    }, lr)
                  ])
                ])
              ]))
            ]),
            s("div", nr, [
              $e(K, {
                onClick: T,
                color: "red",
                class: "ml-2"
              }, {
                default: ke(() => [
                  xe(" Save ")
                ]),
                _: 1
              }),
              $e(ne, { onClick: R }, {
                default: ke(() => [
                  xe(" Cancel ")
                ]),
                _: 1
              })
            ])
          ])
        ], 4)
      ], 512);
    };
  }
}), sr = { class: "px-4 sm:px-6 lg:px-8 text-sm" }, or = { class: "flex flex-wrap" }, ar = { class: "group pr-4 sm:pr-6 lg:pr-8" }, ir = { class: "flex justify-between w-full font-medium" }, rr = { class: "w-6 flex justify-end" }, ur = { class: "hidden group-hover:inline" }, dr = ["onClick", "title"], cr = /* @__PURE__ */ s("svg", {
  class: "h-2 w-2",
  stroke: "currentColor",
  fill: "none",
  viewBox: "0 0 8 8"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-width": "1.5",
    d: "M1 1l6 6m0-6L1 7"
  })
], -1), fr = [
  cr
], mr = {
  key: 0,
  class: "pt-2"
}, vr = { class: "ml-2" }, hr = { key: 1 }, gr = { class: "pt-2" }, pr = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 text-indigo-700" }, yr = ["onClick"], br = /* @__PURE__ */ s("svg", {
  class: "h-2 w-2",
  stroke: "currentColor",
  fill: "none",
  viewBox: "0 0 8 8"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-width": "1.5",
    d: "M1 1l6 6m0-6L1 7"
  })
], -1), wr = [
  br
], xr = /* @__PURE__ */ s("span", null, "Clear All", -1), kr = [
  xr
], Cn = /* @__PURE__ */ ue({
  __name: "FilterViews",
  props: {
    definitions: null,
    columns: null
  },
  emits: ["done", "change"],
  setup(e, { emit: t }) {
    const l = e, n = f(() => l.columns.filter((b) => b.settings.filters.length > 0));
    function i(b) {
      var p, v;
      return (v = (p = b == null ? void 0 : b[0]) == null ? void 0 : p.value) == null ? void 0 : v.split(",");
    }
    function r(b) {
      let p = l.definitions;
      return fs(b) || (p = p.filter((v) => v.types !== "string")), p;
    }
    function d(b, p) {
      return r(b).find((v) => v.value === p);
    }
    function c(b, p) {
      return ks(d(b.type, p.value), b.type, p);
    }
    function m(b) {
      b.settings.filters = [], t("change", b);
    }
    function h(b, p) {
      b.settings.filters.splice(p, 1), t("change", b);
    }
    function y() {
      l.columns.forEach((b) => {
        b.settings.filters = [], t("change", b);
      }), t("done");
    }
    return (b, p) => (a(), u("div", sr, [
      s("div", or, [
        (a(!0), u(Te, null, Ie(o(n), (v) => (a(), u("fieldset", ar, [
          s("legend", ir, [
            s("span", null, A(o(Ee)(v.name)), 1),
            s("span", rr, [
              s("span", ur, [
                s("button", {
                  onClick: (g) => m(v),
                  title: `Clear all ${o(Ee)(v.name)} filters`,
                  class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-red-600 hover:bg-red-200 hover:text-red-500 focus:outline-none focus:bg-red-500 focus:text-white"
                }, fr, 8, dr)
              ])
            ])
          ]),
          v.meta.isEnum ? (a(), u("div", mr, [
            (a(!0), u(Te, null, Ie(i(v.settings.filters), (g) => (a(), u("div", {
              key: g,
              class: "flex items-center"
            }, [
              s("label", vr, A(g), 1)
            ]))), 128))
          ])) : (a(), u("div", hr, [
            (a(!0), u(Te, null, Ie(v.settings.filters, (g, O) => (a(), u("div", gr, [
              s("span", pr, [
                xe(A(v.name) + " " + A(g.name) + " " + A(c(v, g)) + " ", 1),
                s("button", {
                  type: "button",
                  onClick: (U) => h(v, O),
                  class: "flex-shrink-0 ml-0.5 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 hover:bg-indigo-200 hover:text-indigo-500 focus:outline-none focus:bg-indigo-500 focus:text-white"
                }, wr, 8, yr)
              ])
            ]))), 256))
          ]))
        ]))), 256))
      ]),
      s("div", { class: "flex justify-center pt-4" }, [
        s("button", {
          type: "button",
          onClick: y,
          class: "inline-flex items-center px-2.5 py-1.5 border border-gray-300 shadow-sm text-sm font-medium rounded text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
        }, kr)
      ])
    ]));
  }
}), $r = { class: "bg-white dark:bg-black px-4 pt-5 pb-4 sm:p-6 sm:pb-4" }, Cr = { class: "" }, _r = { class: "mt-3 text-center sm:mt-0 sm:mx-4 sm:text-left" }, Lr = /* @__PURE__ */ s("h3", { class: "text-lg leading-6 font-medium text-gray-900 dark:text-gray-100" }, "Query Preferences", -1), Vr = { class: "mt-4" }, Sr = ["for"], Mr = ["id"], Ar = ["value", "selected"], Tr = { class: "mt-4 flex items-center py-4 border-b border-gray-200 dark:border-gray-800" }, Fr = ["id", "checked"], Ir = ["for"], Dr = { class: "mt-4" }, Or = { class: "pb-2 px-4" }, jr = { class: "" }, Pr = ["id", "value"], Br = ["for"], Rr = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse" }, _n = /* @__PURE__ */ ue({
  __name: "QueryPrefs",
  props: {
    id: { default: "QueryPrefs" },
    columns: null,
    prefs: null,
    maxLimit: null
  },
  emits: ["done", "save"],
  setup(e, { emit: t }) {
    const l = e, { autoQueryGridDefaults: n } = St(), i = M({});
    hl(() => i.value = Object.assign({
      take: n.value.take,
      selectedColumns: []
    }, l.prefs));
    const r = [10, 25, 50, 100, 250, 500, 1e3];
    function d() {
      t("done");
    }
    function c() {
      t("save", i.value);
    }
    return (m, h) => {
      const y = X("PrimaryButton"), b = X("SecondaryButton"), p = X("ModalDialog");
      return a(), se(p, {
        id: e.id,
        onDone: d,
        "size-class": "w-full sm:max-w-prose"
      }, {
        default: ke(() => [
          s("div", $r, [
            s("div", Cr, [
              s("div", _r, [
                Lr,
                s("div", Vr, [
                  s("label", {
                    for: `${e.id}-take`,
                    class: "block text-sm font-medium text-gray-700 dark:text-gray-300"
                  }, "Results per page", 8, Sr),
                  kt(s("select", {
                    id: `${e.id}-take`,
                    "onUpdate:modelValue": h[0] || (h[0] = (v) => i.value.take = v),
                    class: "mt-1 block w-full pl-3 pr-10 py-2 text-base bg-white dark:bg-black border-gray-300 dark:border-gray-700 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                  }, [
                    (a(!0), u(Te, null, Ie(r.filter((v) => l.maxLimit == null || v <= l.maxLimit), (v) => (a(), u("option", {
                      value: v,
                      selected: v === i.value.take
                    }, A(v), 9, Ar))), 256))
                  ], 8, Mr), [
                    [lo, i.value.take]
                  ])
                ]),
                s("div", Tr, [
                  s("input", {
                    type: "radio",
                    id: `${e.id}-allColumns`,
                    onClick: h[1] || (h[1] = (v) => i.value.selectedColumns = []),
                    checked: i.value.selectedColumns.length === 0,
                    class: "focus:ring-indigo-500 h-4 w-4 bg-white dark:bg-black text-indigo-600 dark:text-indigo-400 border-gray-300 dark:border-gray-700"
                  }, null, 8, Fr),
                  s("label", {
                    class: "ml-3 block text-gray-700 dark:text-gray-300",
                    for: `${e.id}-allColumns`
                  }, "View all columns", 8, Ir)
                ]),
                s("div", Dr, [
                  s("div", Or, [
                    s("div", jr, [
                      (a(!0), u(Te, null, Ie(e.columns, (v) => (a(), u("div", {
                        key: v.name,
                        class: "flex items-center"
                      }, [
                        kt(s("input", {
                          type: "checkbox",
                          id: v.name,
                          value: v.name,
                          "onUpdate:modelValue": h[2] || (h[2] = (g) => i.value.selectedColumns = g),
                          class: "h-4 w-4 bg-white dark:bg-black border-gray-300 dark:border-gray-700 rounded text-indigo-600 dark:text-indigo-400 focus:ring-indigo-500"
                        }, null, 8, Pr), [
                          [Xl, i.value.selectedColumns]
                        ]),
                        s("label", {
                          for: v.name,
                          class: "ml-3"
                        }, A(v.name), 9, Br)
                      ]))), 128))
                    ])
                  ])
                ])
              ])
            ])
          ]),
          s("div", Rr, [
            $e(y, {
              onClick: c,
              color: "red",
              class: "ml-2"
            }, {
              default: ke(() => [
                xe(" Save ")
              ]),
              _: 1
            }),
            $e(b, { onClick: d }, {
              default: ke(() => [
                xe(" Cancel ")
              ]),
              _: 1
            })
          ])
        ]),
        _: 1
      }, 8, ["id"]);
    };
  }
}), Hr = { key: 0 }, Er = { key: 1 }, zr = {
  key: 2,
  class: "pt-1"
}, Nr = { key: 0 }, Ur = { key: 1 }, qr = { key: 3 }, Qr = { class: "pl-1 pt-1 flex flex-wrap" }, Kr = { class: "flex mt-1" }, Zr = ["title"], Gr = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("g", {
    "stroke-width": "1.5",
    fill: "none"
  }, [
    /* @__PURE__ */ s("path", {
      d: "M9 3H3.6a.6.6 0 0 0-.6.6v16.8a.6.6 0 0 0 .6.6H9M9 3v18M9 3h6M9 21h6m0-18h5.4a.6.6 0 0 1 .6.6v16.8a.6.6 0 0 1-.6.6H15m0-18v18",
      stroke: "currentColor"
    })
  ])
], -1), Wr = [
  Gr
], Jr = ["disabled"], Xr = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6l6 6zM6 6h2v12H6z",
    fill: "currentColor"
  })
], -1), Yr = [
  Xr
], eu = ["disabled"], tu = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M15.41 7.41L14 6l-6 6l6 6l1.41-1.41L10.83 12z",
    fill: "currentColor"
  })
], -1), lu = [
  tu
], nu = ["disabled"], su = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M10 6L8.59 7.41L13.17 12l-4.58 4.59L10 18l6-6z",
    fill: "currentColor"
  })
], -1), ou = [
  su
], au = ["disabled"], iu = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6l-6-6zM16 6h2v12h-2z",
    fill: "currentColor"
  })
], -1), ru = [
  iu
], uu = {
  key: 0,
  class: "flex mt-1"
}, du = { class: "px-4 text-lg text-black dark:text-white" }, cu = { key: 0 }, fu = { key: 1 }, mu = /* @__PURE__ */ s("span", { class: "hidden xl:inline" }, " Showing Results ", -1), vu = { key: 2 }, hu = { class: "flex flex-wrap" }, gu = {
  key: 0,
  class: "pl-2 mt-1"
}, pu = /* @__PURE__ */ s("svg", {
  class: "w-5 h-5",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    fill: "none",
    stroke: "currentColor",
    "stroke-linecap": "round",
    "stroke-linejoin": "round",
    "stroke-width": "2",
    d: "M20 20v-5h-5M4 4v5h5m10.938 2A8.001 8.001 0 0 0 5.07 8m-1.008 5a8.001 8.001  0 0 0 14.868 3"
  })
], -1), yu = [
  pu
], bu = {
  key: 1,
  class: "pl-2 mt-1"
}, wu = /* @__PURE__ */ wl('<svg class="w-5 h-5 mr-1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32"><path d="M28.781 4.405h-10.13V2.018L2 4.588v22.527l16.651 2.868v-3.538h10.13A1.162 1.162 0 0 0 30 25.349V5.5a1.162 1.162 0 0 0-1.219-1.095zm.16 21.126H18.617l-.017-1.889h2.487v-2.2h-2.506l-.012-1.3h2.518v-2.2H18.55l-.012-1.3h2.549v-2.2H18.53v-1.3h2.557v-2.2H18.53v-1.3h2.557v-2.2H18.53v-2h10.411z" fill="#20744a" fill-rule="evenodd"></path><path fill="#20744a" d="M22.487 7.439h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 10.94h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 14.441h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 17.942h4.323v2.2h-4.323z"></path><path fill="#20744a" d="M22.487 21.443h4.323v2.2h-4.323z"></path><path fill="#fff" fill-rule="evenodd" d="M6.347 10.673l2.146-.123l1.349 3.709l1.594-3.862l2.146-.123l-2.606 5.266l2.606 5.279l-2.269-.153l-1.532-4.024l-1.533 3.871l-2.085-.184l2.422-4.663l-2.238-4.993z"></path></svg><span class="text-green-900 dark:text-green-100">Excel</span>', 2), xu = [
  wu
], ku = {
  key: 2,
  class: "pl-2 mt-1"
}, $u = {
  key: 0,
  class: "w-5 h-5 mr-1 text-green-600 dark:text-green-400",
  fill: "none",
  stroke: "currentColor",
  viewBox: "0 0 24 24",
  xmlns: "http://www.w3.org/2000/svg"
}, Cu = /* @__PURE__ */ s("path", {
  "stroke-linecap": "round",
  "stroke-linejoin": "round",
  "stroke-width": "2",
  d: "M5 13l4 4L19 7"
}, null, -1), _u = [
  Cu
], Lu = {
  key: 1,
  class: "w-5 h-5 mr-1",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, Vu = /* @__PURE__ */ s("g", { fill: "none" }, [
  /* @__PURE__ */ s("path", {
    d: "M8 4v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V7.242a2 2 0 0 0-.602-1.43L16.083 2.57A2 2 0 0 0 14.685 2H10a2 2 0 0 0-2 2z",
    stroke: "currentColor",
    "stroke-width": "2",
    "stroke-linecap": "round",
    "stroke-linejoin": "round"
  }),
  /* @__PURE__ */ s("path", {
    d: "M16 18v2a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V9a2 2 0 0 1 2-2h2",
    stroke: "currentColor",
    "stroke-width": "2",
    "stroke-linecap": "round",
    "stroke-linejoin": "round"
  })
], -1), Su = [
  Vu
], Mu = /* @__PURE__ */ s("span", { class: "whitespace-nowrap" }, "Copy URL", -1), Au = {
  key: 3,
  class: "pl-2 mt-1"
}, Tu = /* @__PURE__ */ s("svg", {
  class: "w-5 h-5",
  xmlns: "http://www.w3.org/2000/svg",
  "aria-hidden": "true",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    fill: "currentColor",
    d: "M6.78 2.72a.75.75 0 0 1 0 1.06L4.56 6h8.69a7.75 7.75 0 1 1-7.75 7.75a.75.75 0 0 1 1.5 0a6.25 6.25 0 1 0 6.25-6.25H4.56l2.22 2.22a.75.75 0 1 1-1.06 1.06l-3.5-3.5a.75.75 0 0 1 0-1.06l3.5-3.5a.75.75 0 0 1 1.06 0Z"
  })
], -1), Fu = [
  Tu
], Iu = {
  key: 4,
  class: "pl-2 mt-1"
}, Du = /* @__PURE__ */ s("svg", {
  class: "flex-none w-5 h-5 mr-2 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  "aria-hidden": "true",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z",
    "clip-rule": "evenodd"
  })
], -1), Ou = { class: "mr-1" }, ju = {
  key: 0,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Pu = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
  "clip-rule": "evenodd"
}, null, -1), Bu = [
  Pu
], Ru = {
  key: 1,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, Hu = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M5 10a1 1 0 011-1h8a1 1 0 110 2H6a1 1 0 01-1-1z",
  "clip-rule": "evenodd"
}, null, -1), Eu = [
  Hu
], zu = {
  key: 5,
  class: "pl-2 mt-1"
}, Nu = ["title"], Uu = /* @__PURE__ */ s("svg", {
  class: "w-5 h-5 mr-1 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-50",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z",
    fill: "currentColor"
  })
], -1), qu = { class: "whitespace-nowrap" }, Qu = { key: 7 }, Ku = {
  key: 0,
  class: "cursor-pointer flex justify-between items-center hover:text-gray-900 dark:hover:text-gray-50"
}, Zu = { class: "mr-1 select-none" }, Gu = {
  key: 1,
  class: "flex justify-between items-center"
}, Wu = { class: "mr-1 select-none" }, Ju = /* @__PURE__ */ ue({
  __name: "AutoQueryGrid",
  props: {
    filterDefinitions: null,
    id: { default: "AutoQueryGrid" },
    apis: null,
    type: null,
    prefs: null,
    deny: null,
    hide: null,
    selectedColumns: null,
    toolbarButtonClass: null,
    tableStyle: null,
    gridClass: null,
    grid2Class: null,
    grid3Class: null,
    grid4Class: null,
    tableClass: null,
    theadClass: null,
    tbodyClass: null,
    theadRowClass: null,
    theadCellClass: null,
    headerTitle: null,
    headerTitles: null,
    visibleFrom: null,
    rowClass: null,
    rowStyle: null,
    apiPrefs: null,
    canFilter: null,
    disableKeyBindings: null,
    configureField: null,
    skip: { default: 0 },
    create: { type: Boolean },
    edit: null,
    filters: null
  },
  emits: ["headerSelected", "rowSelected"],
  setup(e, { expose: t, emit: l }) {
    const n = e, { config: i, autoQueryGridDefaults: r } = St(), d = r, c = qe("client"), m = i.value.storage, h = "filtering,queryString,queryFilters".split(","), y = "copyApiUrl,downloadCsv,filtersView,newItem,pagingInfo,pagingNav,preferences,refresh,resetPreferences,toolbar".split(","), b = f(() => n.deny ? bt(h, n.deny) : bt(h, d.value.deny)), p = f(() => n.hide ? bt(y, n.hide) : bt(y, d.value.hide));
    function v($) {
      return b.value[$];
    }
    function g($) {
      return p.value[$];
    }
    const O = f(() => n.tableStyle ?? d.value.tableStyle), U = f(() => n.gridClass ?? he.getGridClass(O.value)), Y = f(() => n.grid2Class ?? he.getGrid2Class(O.value)), R = f(() => n.grid3Class ?? he.getGrid3Class(O.value)), N = f(() => n.grid4Class ?? he.getGrid4Class(O.value)), T = f(() => n.tableClass ?? he.getTableClass(O.value)), J = f(() => n.theadClass ?? he.getTheadClass(O.value)), j = f(() => n.theadRowClass ?? he.getTheadRowClass(O.value)), L = f(() => n.theadCellClass ?? he.getTheadCellClass(O.value)), q = f(() => n.toolbarButtonClass ?? he.toolbarButtonClass);
    function D($, I) {
      var Ae;
      if (n.rowClass)
        return n.rowClass($, I);
      const ce = !!we.value.AnyUpdate, pe = ((Ae = ve.value) != null && Ae.name ? be($, ve.value.name) : null) == _.value;
      return he.getTableRowClass(n.tableStyle, I, pe, ce);
    }
    const K = Yl(), ne = f(() => {
      var $;
      return Dl((($ = we.value.AnyQuery.viewModel) == null ? void 0 : $.name) || we.value.AnyQuery.dataModel.name);
    }), ee = f(() => {
      const $ = Object.keys(K).map((I) => I.toLowerCase());
      return Je(ne.value).filter((I) => $.includes(I.name.toLowerCase()) || $.includes(I.name.toLowerCase() + "-header")).map((I) => I.name);
    });
    function te() {
      let $ = gt(n.selectedColumns);
      return $.length > 0 ? $ : ee.value.length > 0 ? ee.value : [];
    }
    const S = f(() => {
      let I = te().map((ae) => ae.toLowerCase());
      const ce = Je(ne.value);
      return I.length > 0 ? I.map((ae) => ce.find((pe) => pe.name.toLowerCase() === ae)).filter((ae) => ae != null) : ce;
    }), le = f(() => {
      let $ = S.value.map((ce) => ce.name), I = gt(fe.value.selectedColumns).map((ce) => ce.toLowerCase());
      return I.length > 0 ? $.filter((ce) => I.includes(ce.toLowerCase())) : $;
    }), x = M([]), W = M(new We()), E = M(new We()), G = M(), C = M(!1), _ = M(), B = M(), de = M(!1), F = M(), V = M(n.skip), re = M(!1), ye = 25, fe = M({ take: ye }), H = M(!1), P = f(() => x.value.some(($) => $.settings.filters.length > 0 || !!$.settings.sort) || fe.value.selectedColumns), me = f(() => x.value.map(($) => $.settings.filters.length).reduce(($, I) => $ + I, 0)), Ce = f(() => {
      var $;
      return Je(Dl(Mt.value || (($ = we.value.AnyQuery) == null ? void 0 : $.dataModel.name)));
    }), ve = f(() => {
      var $;
      return el(Dl(Mt.value || (($ = we.value.AnyQuery) == null ? void 0 : $.dataModel.name)));
    }), Ve = f(() => fe.value.take ?? ye), Fe = f(() => W.value.response ? be(W.value.response, "results") : []), z = f(() => {
      var $;
      return (($ = W.value.response) == null ? void 0 : $.total) ?? Fe.value.length ?? 0;
    }), Q = f(() => V.value > 0), oe = f(() => V.value > 0), ge = f(() => Fe.value.length >= Ve.value), Se = f(() => Fe.value.length >= Ve.value), Oe = M(), Re = M(), Me = {
      NoQuery: "No Query API was found"
    };
    t({ update: et, search: Vn, createRequestArgs: Fl, reset: Pn, createDone: zt, createSave: jl, editDone: Et, editSave: al, forceUpdate: Ln, setEdit: Us, edit: B });
    function De($) {
      if ($) {
        if (n.canFilter)
          return n.canFilter($);
        const I = Ce.value.find((ce) => ce.name.toLowerCase() == $.toLowerCase());
        if (I)
          return !ms(I);
      }
      return !1;
    }
    function Qe($) {
      v("queryString") && ln($);
    }
    async function je($) {
      V.value += $, V.value < 0 && (V.value = 0);
      const I = Math.floor(z.value / Ve.value) * Ve.value;
      V.value > I && (V.value = I), Qe({ skip: V.value || void 0 }), await et();
    }
    async function Ye($, I) {
      var pe, Ae;
      if (B.value = null, _.value = I, !$ || !I)
        return;
      let ce = Gt(we.value.AnyQuery, { [$]: I });
      const ae = await c.api(ce);
      if (ae.succeeded) {
        let He = (pe = be(ae.response, "results")) == null ? void 0 : pe[0];
        He || console.warn(`API ${(Ae = we.value.AnyQuery) == null ? void 0 : Ae.request.name}(${$}:${I}) returned no results`), B.value = He;
      }
    }
    async function Ht($, I) {
      var pe;
      l("rowSelected", $, I);
      const ce = (pe = ve.value) == null ? void 0 : pe.name, ae = ce ? be($, ce) : null;
      !ce || !ae || (Qe({ edit: ae }), Ye(ce, ae));
    }
    function mt($, I) {
      var ae;
      if (!v("filtering"))
        return;
      let ce = I.target;
      if (De($) && (ce == null ? void 0 : ce.tagName) !== "TD") {
        let pe = (ae = ce == null ? void 0 : ce.closest("TABLE")) == null ? void 0 : ae.getBoundingClientRect(), Ae = x.value.find((He) => He.name.toLowerCase() == $.toLowerCase());
        if (Ae && pe) {
          let He = 318, at = pe.x + He + 10;
          F.value = {
            column: Ae,
            topLeft: {
              x: Math.max(Math.floor(I.clientX + He / 2), at),
              y: pe.y + 45
            }
          };
        }
      }
      l("headerSelected", $, I);
    }
    function ll() {
      F.value = null;
    }
    async function nl($) {
      var ce;
      let I = (ce = F.value) == null ? void 0 : ce.column;
      I && (I.settings = $, m.setItem(sl(I.name), JSON.stringify(I.settings)), await et()), F.value = null;
    }
    async function zs($) {
      m.setItem(sl($.name), JSON.stringify($.settings)), await et();
    }
    async function Ns($) {
      de.value = !1, fe.value = $, m.setItem(Il(), JSON.stringify($)), await et();
    }
    function Us($) {
      Object.assign(B.value, $), Ln();
    }
    function Ln() {
      var I, ce, ae;
      (I = Oe.value) == null || I.forceUpdate(), (ce = Re.value) == null || ce.forceUpdate();
      const $ = Be();
      (ae = $ == null ? void 0 : $.proxy) == null || ae.$forceUpdate();
    }
    async function et() {
      await Vn(Fl());
    }
    async function qs() {
      await et();
    }
    async function Vn($) {
      const I = we.value.AnyQuery;
      if (!I) {
        console.error(Me.NoQuery);
        return;
      }
      let ce = Gt(I, $), ae = Wn((He) => {
        W.value.response = W.value.error = void 0, H.value = He;
      }), pe = await c.api(ce);
      ae(), xt(() => W.value = pe);
      let Ae = be(pe.response, "results") || [];
      !pe.succeeded || Ae.label == 0;
    }
    function Fl() {
      let $ = {
        include: "total",
        take: Ve.value
      }, I = gt(fe.value.selectedColumns || n.selectedColumns);
      if (I.length > 0) {
        let ae = ve.value;
        ae && !I.includes(ae.name) && (I = [ae.name, ...I]);
        const pe = Ce.value, Ae = [];
        I.forEach((He) => {
          var il;
          const at = pe.find((Pe) => Pe.name.toLowerCase() == He.toLowerCase());
          (il = at == null ? void 0 : at.ref) != null && il.selfId && Ae.push(at.ref.selfId), be(K, He) && Ae.push(...pe.filter((Pe) => {
            var vt, Nt;
            return ((Nt = (vt = Pe.ref) == null ? void 0 : vt.selfId) == null ? void 0 : Nt.toLowerCase()) == He.toLowerCase();
          }).map((Pe) => Pe.name));
        }), Ae.forEach((He) => {
          I.includes(He) || I.push(He);
        }), $.fields = I.join(",");
      }
      let ce = [];
      if (x.value.forEach((ae) => {
        ae.settings.sort && ce.push((ae.settings.sort === "DESC" ? "-" : "") + ae.name), ae.settings.filters.forEach((pe) => {
          let Ae = pe.key.replace("%", ae.name);
          $[Ae] = pe.value;
        });
      }), n.filters && Object.keys(n.filters).forEach((ae) => {
        $[ae] = n.filters[ae];
      }), v("queryString") && v("queryFilters")) {
        const ae = location.search ? location.search : location.hash.includes("?") ? "?" + dl(location.hash, "?") : "";
        let pe = El(ae);
        if (Object.keys(pe).forEach((Ae) => {
          S.value.find((at) => at.name.toLowerCase() === Ae.toLowerCase()) && ($[Ae] = pe[Ae]);
        }), typeof pe.skip < "u") {
          const Ae = parseInt(pe.skip);
          isNaN(Ae) || (V.value = $.skip = Ae);
        }
      }
      return typeof $.skip > "u" && V.value > 0 && ($.skip = V.value), ce.length > 0 && ($.orderBy = ce.join(",")), $;
    }
    function Qs() {
      const $ = Sn("csv");
      Ul($), typeof window < "u" && window.open($);
    }
    function Ks() {
      const $ = Sn("json");
      Ul($), re.value = !0, setTimeout(() => re.value = !1, 3e3);
    }
    function Sn($ = "json") {
      var Ae;
      const I = Fl(), ce = `/api/${(Ae = we.value.AnyQuery) == null ? void 0 : Ae.request.name}`, ae = yo(c.baseUrl, Kt(ce, { ...I, jsconfig: "edv" }));
      return ae.indexOf("?") >= 0 ? xl(ae, "?") + "." + $ + "?" + dl(ae, "?") : ae + ".json";
    }
    async function Zs() {
      x.value.forEach(($) => {
        $.settings = { filters: [] }, m.removeItem(sl($.name));
      }), fe.value = { take: ye }, m.removeItem(Il()), await et();
    }
    function Gs() {
      C.value = !0, Qe({ create: null });
    }
    const Mt = f(() => Bt(n.type)), dt = f(() => {
      var $;
      return Mt.value || (($ = we.value.AnyQuery) == null ? void 0 : $.dataModel.name);
    }), Il = () => {
      var $;
      return `${n.id}/ApiPrefs/${Mt.value || (($ = we.value.AnyQuery) == null ? void 0 : $.dataModel.name)}`;
    }, sl = ($) => {
      var I;
      return `Column/${n.id}:${Mt.value || ((I = we.value.AnyQuery) == null ? void 0 : I.dataModel.name)}.${$}`;
    }, { metadataApi: Mn, typeOf: Dl, apiOf: An, filterDefinitions: Ws } = ot(), { invalidAccessMessage: Ol } = kn(), Tn = f(() => n.filterDefinitions || Ws.value), we = f(() => {
      let $ = gt(n.apis);
      return $.length > 0 ? Ot.from($.map((I) => An(I)).filter((I) => I != null).map((I) => I)) : Ot.forType(Mt.value, Mn.value);
    }), ol = ($) => `<span class="text-yellow-700">${$}</span>`, Fn = f(() => {
      if (!Mn.value)
        return ol(`AppMetadata not loaded, see <a class="${bl.blue}" href="https://docs.servicestack.net/vue/use-metadata" target="_blank">useMetadata()</a>`);
      let I = gt(n.apis).map((ae) => An(ae) == null ? ae : null).filter((ae) => ae != null);
      if (I.length > 0)
        return ol(`Unknown API${I.length > 1 ? "s" : ""}: ${I.join(", ")}`);
      let ce = we.value;
      return ce.empty ? ol("Mising DataModel in property 'type' or AutoQuery APIs to use in property 'apis'") : ce.AnyQuery ? null : ol(Me.NoQuery);
    }), In = f(() => we.value.AnyQuery && Ol(we.value.AnyQuery)), Dn = f(() => we.value.Create && Ol(we.value.Create)), On = f(() => we.value.AnyUpdate && Ol(we.value.AnyUpdate)), Js = f(() => vl(we.value.Create));
    f(() => vl(we.value.AnyUpdate));
    const jn = f(() => vl(we.value.Delete));
    function Et() {
      B.value = null, _.value = null, Qe({ edit: void 0 });
    }
    function zt() {
      C.value = !1, Qe({ create: void 0 });
    }
    async function al() {
      await et(), Et();
    }
    async function jl() {
      await et(), zt();
    }
    function Pn() {
      var ce;
      W.value = new We(), E.value = new We(), C.value = !1, _.value = null, B.value = null, de.value = !1, F.value = null, V.value = n.skip, re.value = !1, fe.value = { take: ye }, H.value = !1;
      const $ = n.prefs || yl(m.getItem(Il()));
      $ && (fe.value = $), x.value = S.value.map((ae) => ({
        name: ae.name,
        type: ae.type,
        meta: ae,
        settings: Object.assign(
          {
            filters: []
          },
          yl(m.getItem(sl(ae.name)))
        )
      })), isNaN(n.skip) || (V.value = n.skip);
      let I = (ce = ve.value) == null ? void 0 : ce.name;
      if (v("queryString")) {
        const ae = location.search ? location.search : location.hash.includes("?") ? "?" + dl(location.hash, "?") : "";
        let pe = El(ae);
        typeof pe.create < "u" ? C.value = typeof pe.create < "u" : I && (typeof pe.edit == "string" || typeof pe.edit == "number") && Ye(I, pe.edit);
      }
      n.create === !0 && (C.value = !0), I && n.edit != null && Ye(I, n.edit);
    }
    return Xe(async () => {
      Pn(), await et();
    }), ($, I) => {
      const ce = X("Alert"), ae = X("EnsureAccessDialog"), pe = X("AutoCreateForm"), Ae = X("AutoEditForm"), He = X("ErrorSummary"), at = X("Loading"), Bn = X("SettingsIcons"), il = X("DataGrid");
      return o(Fn) ? (a(), u("div", Hr, [
        $e(ce, { innerHTML: o(Fn) }, null, 8, ["innerHTML"])
      ])) : o(In) ? (a(), u("div", Er, [
        $e(Es, { "invalid-access": o(In) }, null, 8, ["invalid-access"])
      ])) : (a(), u("div", zr, [
        C.value && o(we).Create ? (a(), u("div", Nr, [
          o(Dn) ? (a(), se(ae, {
            key: 0,
            title: `Create ${o(dt)}`,
            "invalid-access": o(Dn),
            "alert-class": "text-yellow-700",
            onDone: zt
          }, null, 8, ["title", "invalid-access"])) : o(K).createform ? Z($.$slots, "createform", {
            key: 1,
            type: o(we).Create.request.name,
            configure: e.configureField,
            done: zt,
            save: jl
          }) : (a(), se(pe, {
            key: 2,
            ref_key: "createForm",
            ref: Oe,
            type: o(we).Create.request.name,
            configure: e.configureField,
            onDone: zt,
            onSave: jl
          }, {
            header: ke(() => [
              Z($.$slots, "formheader", {
                form: "create",
                formInstance: Oe.value,
                apis: o(we),
                type: o(dt)
              })
            ]),
            footer: ke(() => [
              Z($.$slots, "formfooter", {
                form: "create",
                formInstance: Oe.value,
                apis: o(we),
                type: o(dt)
              })
            ]),
            _: 3
          }, 8, ["type", "configure"]))
        ])) : B.value && o(we).AnyUpdate ? (a(), u("div", Ur, [
          o(On) ? (a(), se(ae, {
            key: 0,
            title: `Update ${o(dt)}`,
            "invalid-access": o(On),
            "alert-class": "text-yellow-700",
            onDone: Et
          }, null, 8, ["title", "invalid-access"])) : o(K).editform ? Z($.$slots, "editform", {
            key: 1,
            model: B.value,
            type: o(we).AnyUpdate.request.name,
            deleteType: o(jn) ? o(we).Delete.request.name : null,
            configure: e.configureField,
            done: Et,
            save: al
          }) : (a(), se(Ae, {
            key: 2,
            ref_key: "editForm",
            ref: Re,
            modelValue: B.value,
            "onUpdate:modelValue": I[0] || (I[0] = (Pe) => B.value = Pe),
            type: o(we).AnyUpdate.request.name,
            deleteType: o(jn) ? o(we).Delete.request.name : null,
            configure: e.configureField,
            onDone: Et,
            onSave: al,
            onDelete: al
          }, {
            header: ke(() => [
              Z($.$slots, "formheader", {
                form: "edit",
                formInstance: Re.value,
                apis: o(we),
                type: o(dt),
                model: B.value,
                id: _.value
              })
            ]),
            footer: ke(() => [
              Z($.$slots, "formfooter", {
                form: "edit",
                formInstance: Re.value,
                apis: o(we),
                type: o(dt),
                model: B.value,
                id: _.value
              })
            ]),
            _: 3
          }, 8, ["modelValue", "type", "deleteType", "configure"]))
        ])) : k("", !0),
        o(K).toolbar ? Z($.$slots, "toolbar", { key: 2 }) : g("toolbar") ? (a(), u("div", qr, [
          de.value ? (a(), se(_n, {
            key: 0,
            columns: o(S),
            prefs: fe.value,
            onDone: I[1] || (I[1] = (Pe) => de.value = !1),
            onSave: Ns
          }, null, 8, ["columns", "prefs"])) : k("", !0),
          s("div", Qr, [
            s("div", Kr, [
              g("preferences") ? (a(), u("button", {
                key: 0,
                type: "button",
                class: "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400",
                title: `${o(dt)} Preferences`,
                onClick: I[2] || (I[2] = (Pe) => de.value = !de.value)
              }, Wr, 8, Zr)) : k("", !0),
              g("pagingNav") ? (a(), u("button", {
                key: 1,
                type: "button",
                class: w(["pl-2", o(Q) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "First page",
                disabled: !o(Q),
                onClick: I[3] || (I[3] = (Pe) => je(-o(z)))
              }, Yr, 10, Jr)) : k("", !0),
              g("pagingNav") ? (a(), u("button", {
                key: 2,
                type: "button",
                class: w(["pl-2", o(oe) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Previous page",
                disabled: !o(oe),
                onClick: I[4] || (I[4] = (Pe) => je(-o(Ve)))
              }, lu, 10, eu)) : k("", !0),
              g("pagingNav") ? (a(), u("button", {
                key: 3,
                type: "button",
                class: w(["pl-2", o(ge) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Next page",
                disabled: !o(ge),
                onClick: I[5] || (I[5] = (Pe) => je(o(Ve)))
              }, ou, 10, nu)) : k("", !0),
              g("pagingNav") ? (a(), u("button", {
                key: 4,
                type: "button",
                class: w(["pl-2", o(Se) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                title: "Last page",
                disabled: !o(Se),
                onClick: I[6] || (I[6] = (Pe) => je(o(z)))
              }, ru, 10, au)) : k("", !0)
            ]),
            g("pagingInfo") ? (a(), u("div", uu, [
              s("div", du, [
                H.value ? (a(), u("span", cu, "Querying...")) : k("", !0),
                o(Fe).length ? (a(), u("span", fu, [
                  mu,
                  xe(" " + A(V.value + 1) + " - " + A(Math.min(V.value + o(Fe).length, o(z))) + " ", 1),
                  s("span", null, " of " + A(o(z)), 1)
                ])) : W.value.completed ? (a(), u("span", vu, "No Results")) : k("", !0)
              ])
            ])) : k("", !0),
            s("div", hu, [
              g("refresh") ? (a(), u("div", gu, [
                s("button", {
                  type: "button",
                  onClick: qs,
                  title: "Refresh",
                  class: w(o(q))
                }, yu, 2)
              ])) : k("", !0),
              g("downloadCsv") ? (a(), u("div", bu, [
                s("button", {
                  type: "button",
                  onClick: Qs,
                  title: "Download CSV",
                  class: w(o(q))
                }, xu, 2)
              ])) : k("", !0),
              g("copyApiUrl") ? (a(), u("div", ku, [
                s("button", {
                  type: "button",
                  onClick: Ks,
                  title: "Copy API URL",
                  class: w(o(q))
                }, [
                  re.value ? (a(), u("svg", $u, _u)) : (a(), u("svg", Lu, Su)),
                  Mu
                ], 2)
              ])) : k("", !0),
              o(P) && g("resetPreferences") ? (a(), u("div", Au, [
                s("button", {
                  type: "button",
                  onClick: Zs,
                  title: "Reset Preferences & Filters",
                  class: w(o(q))
                }, Fu, 2)
              ])) : k("", !0),
              g("filtersView") && o(me) > 0 ? (a(), u("div", Iu, [
                s("button", {
                  type: "button",
                  onClick: I[7] || (I[7] = (Pe) => G.value = G.value == "filters" ? null : "filters"),
                  class: w(o(q)),
                  "aria-expanded": "false"
                }, [
                  Du,
                  s("span", Ou, A(o(me)) + " " + A(o(me) == 1 ? "Filter" : "Filters"), 1),
                  G.value != "filters" ? (a(), u("svg", ju, Bu)) : (a(), u("svg", Ru, Eu))
                ], 2)
              ])) : k("", !0),
              g("newItem") && o(we).Create && o(Js) ? (a(), u("div", zu, [
                s("button", {
                  type: "button",
                  onClick: Gs,
                  title: o(dt),
                  class: w(o(q))
                }, [
                  Uu,
                  s("span", qu, "New " + A(o(dt)), 1)
                ], 10, Nu)
              ])) : k("", !0),
              o(K).toolbarbuttons ? Z($.$slots, "toolbarbuttons", {
                key: 6,
                toolbarButtonClass: o(q)
              }) : k("", !0)
            ])
          ])
        ])) : k("", !0),
        G.value == "filters" ? (a(), se(Cn, {
          key: 4,
          class: "border-y border-gray-200 dark:border-gray-800 py-8 my-2",
          definitions: o(Tn),
          columns: x.value,
          onDone: I[8] || (I[8] = (Pe) => G.value = null),
          onChange: zs
        }, null, 8, ["definitions", "columns"])) : k("", !0),
        E.value.error ?? W.value.error ? (a(), se(He, {
          key: 5,
          status: E.value.error ?? W.value.error
        }, null, 8, ["status"])) : H.value ? (a(), se(at, {
          key: 6,
          class: "p-2"
        })) : k("", !0),
        F.value ? (a(), u("div", Qu, [
          $e($n, {
            definitions: o(Tn),
            column: F.value.column,
            "top-left": F.value.topLeft,
            onDone: ll,
            onSave: nl
          }, null, 8, ["definitions", "column", "top-left"])
        ])) : k("", !0),
        o(Fe).length ? (a(), se(il, {
          key: 8,
          id: e.id,
          items: o(Fe),
          type: e.type,
          "selected-columns": o(le),
          class: "mt-1",
          onFiltersChanged: et,
          tableStyle: o(O),
          gridClass: o(U),
          grid2Class: o(Y),
          grid3Class: o(R),
          grid4Class: o(N),
          tableClass: o(T),
          theadClass: o(J),
          theadRowClass: o(j),
          theadCellClass: o(L),
          tbodyClass: e.tbodyClass,
          rowClass: D,
          onRowSelected: Ht,
          rowStyle: e.rowStyle,
          headerTitle: e.headerTitle,
          headerTitles: e.headerTitles,
          visibleFrom: e.visibleFrom,
          onHeaderSelected: mt
        }, en({
          header: ke(({ column: Pe, label: vt }) => {
            var Nt;
            return [
              v("filtering") && De(Pe) ? (a(), u("div", Ku, [
                s("span", Zu, A(vt), 1),
                $e(Bn, {
                  column: x.value.find((Xs) => Xs.name.toLowerCase() === Pe.toLowerCase()),
                  "is-open": ((Nt = F.value) == null ? void 0 : Nt.column.name) === Pe
                }, null, 8, ["column", "is-open"])
              ])) : (a(), u("div", Gu, [
                s("span", Wu, A(vt), 1)
              ]))
            ];
          }),
          _: 2
        }, [
          Ie(Object.keys(o(K)), (Pe) => ({
            name: Pe,
            fn: ke((vt) => [
              Z($.$slots, Pe, It(gl(vt)))
            ])
          }))
        ]), 1032, ["id", "items", "type", "selected-columns", "tableStyle", "gridClass", "grid2Class", "grid3Class", "grid4Class", "tableClass", "theadClass", "theadRowClass", "theadCellClass", "tbodyClass", "rowStyle", "headerTitle", "headerTitles", "visibleFrom"])) : k("", !0)
      ]));
    };
  }
}), Xu = { class: "flex" }, Yu = {
  key: 0,
  class: "w-4 h-4",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, ed = /* @__PURE__ */ s("g", { fill: "none" }, [
  /* @__PURE__ */ s("path", {
    d: "M3 4a1 1 0 0 1 1-1h16a1 1 0 0 1 1 1v2.586a1 1 0 0 1-.293.707l-6.414 6.414a1 1 0 0 0-.293.707V17l-4 4v-6.586a1 1 0 0 0-.293-.707L3.293 7.293A1 1 0 0 1 3 6.586V4z",
    stroke: "currentColor",
    "stroke-width": "2",
    "stroke-linecap": "round",
    "stroke-linejoin": "round"
  })
], -1), td = [
  ed
], ld = /* @__PURE__ */ s("path", {
  d: "M505.5 658.7c3.2 4.4 9.7 4.4 12.9 0l178-246c3.8-5.3 0-12.7-6.5-12.7H643c-10.2 0-19.9 4.9-25.9 13.2L512 558.6L406.8 413.2c-6-8.3-15.6-13.2-25.9-13.2H334c-6.5 0-10.3 7.4-6.5 12.7l178 246z",
  fill: "currentColor"
}, null, -1), nd = /* @__PURE__ */ s("path", {
  d: "M880 112H144c-17.7 0-32 14.3-32 32v736c0 17.7 14.3 32 32 32h736c17.7 0 32-14.3 32-32V144c0-17.7-14.3-32-32-32zm-40 728H184V184h656v656z",
  fill: "currentColor"
}, null, -1), sd = [
  ld,
  nd
], od = {
  key: 2,
  class: "w-4 h-4",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20"
}, ad = /* @__PURE__ */ s("g", { fill: "none" }, [
  /* @__PURE__ */ s("path", {
    d: "M8.998 4.71L6.354 7.354a.5.5 0 1 1-.708-.707L9.115 3.18A.499.499 0 0 1 9.498 3H9.5a.5.5 0 0 1 .354.147l.01.01l3.49 3.49a.5.5 0 1 1-.707.707l-2.65-2.649V16.5a.5.5 0 0 1-1 0V4.71z",
    fill: "currentColor"
  })
], -1), id = [
  ad
], rd = {
  key: 3,
  class: "w-4 h-4",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20"
}, ud = /* @__PURE__ */ s("g", { fill: "none" }, [
  /* @__PURE__ */ s("path", {
    d: "M10.002 15.29l2.645-2.644a.5.5 0 0 1 .707.707L9.886 16.82a.5.5 0 0 1-.384.179h-.001a.5.5 0 0 1-.354-.147l-.01-.01l-3.49-3.49a.5.5 0 1 1 .707-.707l2.648 2.649V3.5a.5.5 0 0 1 1 0v11.79z",
    fill: "currentColor"
  })
], -1), dd = [
  ud
], cd = /* @__PURE__ */ ue({
  __name: "SettingsIcons",
  props: {
    column: null,
    isOpen: { type: Boolean }
  },
  setup(e) {
    return (t, l) => {
      var n, i, r, d, c, m, h;
      return a(), u("div", Xu, [
        (r = (i = (n = e.column) == null ? void 0 : n.settings) == null ? void 0 : i.filters) != null && r.length ? (a(), u("svg", Yu, td)) : (a(), u("svg", {
          key: 1,
          class: w(["w-4 h-4 transition-transform", e.isOpen ? "rotate-180" : ""]),
          xmlns: "http://www.w3.org/2000/svg",
          viewBox: "0 0 1024 1024"
        }, sd, 2)),
        ((c = (d = e.column) == null ? void 0 : d.settings) == null ? void 0 : c.sort) === "ASC" ? (a(), u("svg", od, id)) : ((h = (m = e.column) == null ? void 0 : m.settings) == null ? void 0 : h.sort) === "DESC" ? (a(), u("svg", rd, dd)) : k("", !0)
      ]);
    };
  }
}), fd = /* @__PURE__ */ ue({
  __name: "EnsureAccessDialog",
  props: {
    title: null,
    subtitle: null,
    invalidAccess: null,
    alertClass: null
  },
  emits: ["done"],
  setup(e) {
    return (t, l) => {
      const n = X("EnsureAccess"), i = X("SlideOver");
      return e.invalidAccess ? (a(), se(i, {
        key: 0,
        title: e.title,
        onDone: l[0] || (l[0] = (r) => t.$emit("done")),
        "content-class": "relative flex-1"
      }, en({
        default: ke(() => [
          $e(n, {
            alertClass: e.alertClass,
            invalidAccess: e.invalidAccess
          }, null, 8, ["alertClass", "invalidAccess"])
        ]),
        _: 2
      }, [
        e.subtitle ? {
          name: "subtitle",
          fn: ke(() => [
            xe(A(e.subtitle), 1)
          ]),
          key: "0"
        } : void 0
      ]), 1032, ["title"])) : k("", !0);
    };
  }
}), md = ["for"], vd = { class: "mt-1 relative rounded-md shadow-sm" }, hd = ["type", "name", "id", "placeholder", "value", "aria-invalid", "aria-describedby"], gd = {
  key: 0,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, pd = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5 text-red-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
    "clip-rule": "evenodd"
  })
], -1), yd = [
  pd
], bd = ["id"], wd = ["id"], xd = {
  inheritAttrs: !1
}, kd = /* @__PURE__ */ ue({
  ...xd,
  __name: "TextInput",
  props: {
    status: null,
    id: null,
    type: null,
    inputClass: null,
    label: null,
    labelClass: null,
    help: null,
    placeholder: null,
    modelValue: null
  },
  setup(e, { expose: t }) {
    const l = e, n = (p) => p.value;
    t({
      focus: r
    });
    const i = M();
    function r() {
      var p;
      (p = i.value) == null || p.focus();
    }
    const d = f(() => l.type || "text"), c = f(() => l.label ?? Ee(nt(l.id))), m = f(() => l.placeholder ?? c.value);
    let h = qe("ApiState", void 0);
    const y = f(() => ft.call({ responseStatus: l.status ?? (h == null ? void 0 : h.error.value) }, l.id)), b = f(() => [lt.base, y.value ? lt.invalid : lt.valid, l.inputClass]);
    return (p, v) => (a(), u("div", {
      class: w([p.$attrs.class])
    }, [
      Z(p.$slots, "header", Le({
        inputElement: i.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, p.$attrs)),
      o(c) ? (a(), u("label", {
        key: 0,
        for: e.id,
        class: w(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, A(o(c)), 11, md)) : k("", !0),
      s("div", vd, [
        s("input", Le({
          ref_key: "inputElement",
          ref: i,
          type: o(d),
          name: e.id,
          id: e.id,
          class: o(b),
          placeholder: o(m),
          value: e.modelValue,
          onInput: v[0] || (v[0] = (g) => p.$emit("update:modelValue", n(g.target))),
          "aria-invalid": o(y) != null,
          "aria-describedby": `${e.id}-error`,
          step: "any"
        }, o(ut)(p.$attrs, ["class"])), null, 16, hd),
        o(y) ? (a(), u("div", gd, yd)) : k("", !0)
      ]),
      o(y) ? (a(), u("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, A(o(y)), 9, bd)) : e.help ? (a(), u("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, A(e.help), 9, wd)) : k("", !0),
      Z(p.$slots, "footer", Le({
        inputElement: i.value,
        id: e.id,
        modelValue: e.modelValue,
        status: e.status
      }, p.$attrs))
    ], 2));
  }
}), $d = ["for"], Cd = { class: "mt-1 relative rounded-md shadow-sm" }, _d = ["name", "id", "placeholder", "aria-invalid", "aria-describedby"], Ld = ["id"], Vd = ["id"], Sd = {
  inheritAttrs: !1
}, Md = /* @__PURE__ */ ue({
  ...Sd,
  __name: "TextareaInput",
  props: {
    status: null,
    id: null,
    inputClass: null,
    label: null,
    labelClass: null,
    help: null,
    placeholder: null,
    modelValue: null
  },
  setup(e) {
    const t = e, l = (m) => m.value, n = f(() => t.label ?? Ee(nt(t.id))), i = f(() => t.placeholder ?? n.value);
    let r = qe("ApiState", void 0);
    const d = f(() => ft.call({ responseStatus: t.status ?? (r == null ? void 0 : r.error.value) }, t.id)), c = f(() => ["shadow-sm " + lt.base, d.value ? "text-red-900 focus:ring-red-500 focus:border-red-500 border-red-300" : "text-gray-900 " + lt.valid, t.inputClass]);
    return (m, h) => (a(), u("div", {
      class: w([m.$attrs.class])
    }, [
      o(n) ? (a(), u("label", {
        key: 0,
        for: e.id,
        class: w(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, A(o(n)), 11, $d)) : k("", !0),
      s("div", Cd, [
        s("textarea", Le({
          name: e.id,
          id: e.id,
          class: o(c),
          placeholder: o(i),
          onInput: h[0] || (h[0] = (y) => m.$emit("update:modelValue", l(y.target))),
          "aria-invalid": o(d) != null,
          "aria-describedby": `${e.id}-error`
        }, o(ut)(m.$attrs, ["class"])), A(e.modelValue), 17, _d)
      ]),
      o(d) ? (a(), u("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, A(o(d)), 9, Ld)) : e.help ? (a(), u("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, A(e.help), 9, Vd)) : k("", !0)
    ], 2));
  }
}), Ad = ["for"], Td = ["id", "name", "value", "aria-invalid", "aria-describedby"], Fd = ["value"], Id = ["id"], Dd = {
  inheritAttrs: !1
}, Od = /* @__PURE__ */ ue({
  ...Dd,
  __name: "SelectInput",
  props: {
    status: null,
    id: null,
    modelValue: null,
    inputClass: null,
    label: null,
    labelClass: null,
    options: null,
    values: null,
    entries: null
  },
  setup(e) {
    const t = e, l = (c) => c.value, n = f(() => t.label ?? Ee(nt(t.id)));
    let i = qe("ApiState", void 0);
    const r = f(() => ft.call({ responseStatus: t.status ?? (i == null ? void 0 : i.error.value) }, t.id)), d = f(() => t.entries || (t.values ? t.values.map((c) => ({ key: c, value: c })) : t.options ? Object.keys(t.options).map((c) => ({ key: c, value: t.options[c] })) : []));
    return (c, m) => (a(), u("div", {
      class: w([c.$attrs.class])
    }, [
      o(n) ? (a(), u("label", {
        key: 0,
        for: e.id,
        class: w(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, A(o(n)), 11, Ad)) : k("", !0),
      s("select", Le({
        id: e.id,
        name: e.id,
        class: [
          "mt-1 block w-full pl-3 pr-10 py-2 text-base focus:outline-none sm:text-sm rounded-md dark:text-white dark:bg-gray-900 dark:border-gray-600",
          o(r) ? "border-red-300 text-red-900 focus:ring-red-500 focus:border-red-500" : "border-gray-300 text-gray-900 focus:ring-indigo-500 focus:border-indigo-500",
          e.inputClass
        ],
        value: e.modelValue,
        onInput: m[0] || (m[0] = (h) => c.$emit("update:modelValue", l(h.target))),
        "aria-invalid": o(r) != null,
        "aria-describedby": `${e.id}-error`
      }, o(ut)(c.$attrs, ["class"])), [
        (a(!0), u(Te, null, Ie(o(d), (h) => (a(), u("option", {
          value: h.key
        }, A(h.value), 9, Fd))), 256))
      ], 16, Td),
      o(r) ? (a(), u("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, A(o(r)), 9, Id)) : k("", !0)
    ], 2));
  }
}), jd = { class: "flex items-center h-5" }, Pd = ["id", "name", "checked"], Bd = { class: "ml-3 text-sm" }, Rd = ["for"], Hd = {
  key: 0,
  class: "mt-2 text-sm text-red-500",
  id: "`${id}-error`"
}, Ed = {
  key: 1,
  class: "mt-2 text-sm text-gray-500",
  id: "`${id}-description`"
}, zd = {
  inheritAttrs: !1
}, Nd = /* @__PURE__ */ ue({
  ...zd,
  __name: "CheckboxInput",
  props: {
    modelValue: { type: Boolean },
    status: null,
    id: null,
    inputClass: null,
    label: null,
    labelClass: null,
    help: null
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, n = f(() => l.label ?? Ee(nt(l.id)));
    let i = qe("ApiState", void 0);
    const r = f(() => ft.call({ responseStatus: l.status ?? (i == null ? void 0 : i.error.value) }, l.id));
    return (d, c) => (a(), u("div", {
      class: w(["relative flex items-start", d.$attrs.class])
    }, [
      s("div", jd, [
        s("input", Le({
          id: e.id,
          name: e.id,
          type: "checkbox",
          checked: e.modelValue,
          onInput: c[0] || (c[0] = (m) => d.$emit("update:modelValue", m.target.checked)),
          class: ["focus:ring-indigo-500 h-4 w-4 text-indigo-600 rounded border-gray-300 dark:border-gray-600 dark:bg-gray-800", e.inputClass]
        }, o(ut)(d.$attrs, ["class"])), null, 16, Pd)
      ]),
      s("div", Bd, [
        s("label", {
          for: e.id,
          class: w(`font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, A(o(n)), 11, Rd),
        o(r) ? (a(), u("p", Hd, A(o(r)), 1)) : e.help ? (a(), u("p", Ed, A(e.help), 1)) : k("", !0)
      ])
    ], 2));
  }
}), Ud = ["id"], qd = ["for"], Qd = { class: "mt-1 relative rounded-md shadow-sm" }, Kd = ["id", "name", "value"], Zd = ["onClick"], Gd = { class: "flex flex-wrap pb-1.5" }, Wd = { class: "pt-1.5 pl-1" }, Jd = { class: "inline-flex rounded-full items-center py-0.5 pl-2.5 pr-1 text-sm font-medium bg-indigo-100 dark:bg-indigo-800 text-indigo-700 dark:text-indigo-300" }, Xd = ["onClick"], Yd = /* @__PURE__ */ s("svg", {
  class: "h-2 w-2",
  stroke: "currentColor",
  fill: "none",
  viewBox: "0 0 8 8"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-width": "1.5",
    d: "M1 1l6 6m0-6L1 7"
  })
], -1), ec = [
  Yd
], tc = { class: "pt-1.5 pl-1 shrink" }, lc = ["type", "name", "id", "aria-invalid", "aria-describedby", "onPaste"], nc = ["id"], sc = ["onMouseover", "onClick"], oc = { class: "block truncate" }, ac = {
  key: 1,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, ic = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5 text-red-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
    "clip-rule": "evenodd"
  })
], -1), rc = [
  ic
], uc = ["id"], dc = ["id"], cc = {
  inheritAttrs: !1
}, fc = /* @__PURE__ */ ue({
  ...cc,
  __name: "TagInput",
  props: {
    status: null,
    id: null,
    type: null,
    inputClass: null,
    label: null,
    labelClass: null,
    help: null,
    modelValue: { default: () => [] },
    delimiters: { default: () => [","] },
    allowableValues: null,
    string: { type: Boolean },
    converter: null
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e;
    function n(x) {
      return l.converter ? l.converter(x) : x;
    }
    const i = f(() => Ue(n(l.modelValue), (x) => typeof x == "string" ? x.trim().length == 0 ? [] : x.split(",") : x) || []), r = M(), d = M(!1), c = f(() => !l.allowableValues || l.allowableValues.length == 0 ? [] : l.allowableValues.filter((x) => !i.value.includes(x) && x.toLowerCase().includes(y.value.toLowerCase())));
    function m(x) {
      r.value = x;
    }
    const h = M(null), y = M(""), b = f(() => l.type || "text"), p = f(() => l.label ?? Ee(nt(l.id)));
    let v = qe("ApiState", void 0);
    const g = f(() => ft.call({ responseStatus: l.status ?? (v == null ? void 0 : v.error.value) }, l.id)), O = f(() => [
      "w-full cursor-text flex flex-wrap sm:text-sm rounded-md dark:text-white dark:bg-gray-900 border focus-within:border-transparent focus-within:ring-1 focus-within:outline-none",
      g.value ? "pr-10 border-red-300 text-red-900 placeholder-red-300 focus-within:outline-none focus-within:ring-red-500 focus-within:border-red-500" : "shadow-sm border-gray-300 dark:border-gray-600 focus-within:ring-indigo-500 focus-within:border-indigo-500",
      l.inputClass
    ]), U = (x) => j(i.value.filter((W) => W != x));
    function Y(x) {
      var W;
      document.activeElement === x.target && ((W = h.value) == null || W.focus());
    }
    const R = M();
    function N() {
      d.value = !0, R.value = !0;
    }
    function T() {
      N();
    }
    function J() {
      te(q()), R.value = !1, setTimeout(() => {
        R.value || (d.value = !1);
      }, 200);
    }
    function j(x) {
      const W = l.string ? x.join(",") : x;
      t("update:modelValue", W);
    }
    function L(x) {
      if (x.key == "Backspace" && y.value.length == 0 && i.value.length > 0 && U(i.value[i.value.length - 1]), !(!l.allowableValues || l.allowableValues.length == 0))
        if (x.code == "Escape" || x.code == "Tab")
          d.value = !1;
        else if (x.code == "Home")
          r.value = c.value[0], ne();
        else if (x.code == "End")
          r.value = c.value[c.value.length - 1], ne();
        else if (x.code == "ArrowDown") {
          if (d.value = !0, !r.value)
            r.value = c.value[0];
          else {
            const W = c.value.indexOf(r.value);
            r.value = W + 1 < c.value.length ? c.value[W + 1] : c.value[0];
          }
          ee();
        } else if (x.code == "ArrowUp") {
          if (!r.value)
            r.value = c.value[c.value.length - 1];
          else {
            const W = c.value.indexOf(r.value);
            r.value = W - 1 >= 0 ? c.value[W - 1] : c.value[c.value.length - 1];
          }
          ee();
        } else
          x.code == "Enter" ? r.value && d.value ? (te(r.value), x.preventDefault()) : d.value = !1 : d.value = c.value.length > 0;
    }
    function q() {
      if (y.value.length == 0)
        return "";
      let x = bo(y.value.trim(), ",");
      return x[0] == "," && (x = x.substring(1)), x = x.trim(), x.length == 0 && d.value && c.value.length > 0 ? r.value : x;
    }
    function D(x) {
      const W = q();
      if (W.length > 0) {
        const E = l.delimiters.some((C) => C == x.key);
        if (E && x.preventDefault(), x.key == "Enter" || x.key == "NumpadEnter" || x.key.length == 1 && E) {
          te(W);
          return;
        }
      }
    }
    const K = { behavior: "smooth", block: "nearest", inline: "nearest", scrollMode: "if-needed" };
    function ne() {
      setTimeout(() => {
        let x = pl(`#${l.id}-tag li.active`);
        x && x.scrollIntoView(K);
      }, 0);
    }
    function ee() {
      setTimeout(() => {
        let x = pl(`#${l.id}-tag li.active`);
        x && ("scrollIntoViewIfNeeded" in x ? x.scrollIntoViewIfNeeded(K) : x.scrollIntoView(K));
      }, 0);
    }
    function te(x) {
      if (x.length === 0)
        return;
      const W = Array.from(i.value);
      W.indexOf(x) == -1 && W.push(x), j(W), y.value = "", d.value = !1;
    }
    function S(x) {
      var E;
      const W = (E = x.clipboardData) == null ? void 0 : E.getData("Text");
      le(W);
    }
    function le(x) {
      if (!x)
        return;
      const W = new RegExp(`\\n|\\t|${l.delimiters.join("|")}`), E = Array.from(i.value);
      x.split(W).map((C) => C.trim()).forEach((C) => {
        E.indexOf(C) == -1 && E.push(C);
      }), j(E), y.value = "";
    }
    return (x, W) => (a(), u("div", {
      class: w([x.$attrs.class]),
      id: `${e.id}-tag`,
      onmousemove: "cancelBlur=true"
    }, [
      o(p) ? (a(), u("label", {
        key: 0,
        for: e.id,
        class: w(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
      }, A(o(p)), 11, qd)) : k("", !0),
      s("div", Qd, [
        s("input", {
          type: "hidden",
          id: e.id,
          name: e.id,
          value: o(i).join(",")
        }, null, 8, Kd),
        s("button", {
          class: w(o(O)),
          onClick: Ne(Y, ["prevent"]),
          onFocus: W[2] || (W[2] = (E) => d.value = !0),
          tabindex: "-1"
        }, [
          s("div", Gd, [
            (a(!0), u(Te, null, Ie(o(i), (E) => (a(), u("div", Wd, [
              s("span", Jd, [
                xe(A(E) + " ", 1),
                s("button", {
                  type: "button",
                  onClick: (G) => U(E),
                  class: "flex-shrink-0 ml-1 h-4 w-4 rounded-full inline-flex items-center justify-center text-indigo-400 dark:text-indigo-500 hover:bg-indigo-200 dark:hover:bg-indigo-800 hover:text-indigo-500 dark:hover:text-indigo-400 focus:outline-none focus:bg-indigo-500 focus:text-white dark:focus:text-black"
                }, ec, 8, Xd)
              ])
            ]))), 256)),
            s("div", tc, [
              kt(s("input", Le({
                ref_key: "txtInput",
                ref: h,
                type: o(b),
                role: "combobox",
                "aria-controls": "options",
                "aria-expanded": "false",
                autocomplete: "off",
                spellcheck: "false",
                name: `${e.id}-txt`,
                id: `${e.id}-txt`,
                class: "p-0 dark:bg-transparent rounded-md border-none focus:!border-none focus:!outline-none",
                style: `box-shadow:none !important;width:${y.value.length + 1}ch`,
                "onUpdate:modelValue": W[0] || (W[0] = (E) => y.value = E),
                "aria-invalid": o(g) != null,
                "aria-describedby": `${e.id}-error`,
                onKeydown: L,
                onKeypress: D,
                onPaste: Ne(S, ["prevent", "stop"]),
                onFocus: T,
                onBlur: J,
                onClick: W[1] || (W[1] = (E) => d.value = !0)
              }, o(ut)(x.$attrs, ["class", "required"])), null, 16, lc), [
                [no, y.value]
              ])
            ])
          ])
        ], 42, Zd),
        d.value && o(c).length ? (a(), u("ul", {
          key: 0,
          class: "absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white dark:bg-black py-1 text-base shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm",
          onKeydown: L,
          id: `${e.id}-options`,
          role: "listbox"
        }, [
          (a(!0), u(Te, null, Ie(o(c), (E) => (a(), u("li", {
            class: w([E === r.value ? "active bg-indigo-600 text-white" : "text-gray-900 dark:text-gray-100", "relative cursor-default select-none py-2 pl-3 pr-9"]),
            onMouseover: (G) => m(E),
            onClick: (G) => te(E),
            role: "option",
            tabindex: "-1"
          }, [
            s("span", oc, A(E), 1)
          ], 42, sc))), 256))
        ], 40, nc)) : k("", !0),
        o(g) ? (a(), u("div", ac, rc)) : k("", !0)
      ]),
      o(g) ? (a(), u("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, A(o(g)), 9, uc)) : e.help ? (a(), u("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, A(e.help), 9, dc)) : k("", !0)
    ], 10, Ud));
  }
}), mc = { class: "relative flex-grow mr-2 sm:mr-4" }, vc = ["for"], hc = { class: "block mt-2" }, gc = { class: "sr-only" }, pc = ["multiple", "name", "id", "placeholder", "aria-invalid", "aria-describedby"], yc = {
  key: 0,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
}, bc = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5 text-red-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
    "clip-rule": "evenodd"
  })
], -1), wc = [
  bc
], xc = ["id"], kc = ["id"], $c = { key: 0 }, Cc = ["title"], _c = ["alt", "src"], Lc = {
  key: 1,
  class: "mt-3"
}, Vc = { class: "w-full" }, Sc = { class: "pr-6 align-bottom pb-2" }, Mc = ["title"], Ac = ["src", "onError"], Tc = ["href"], Fc = {
  key: 1,
  class: "overflow-hidden"
}, Ic = { class: "align-top pb-2 whitespace-nowrap" }, Dc = {
  key: 0,
  class: "text-gray-500 dark:text-gray-400 text-sm bg-white dark:bg-black"
}, Oc = /* @__PURE__ */ ue({
  __name: "FileInput",
  props: {
    multiple: { type: Boolean },
    status: null,
    id: null,
    inputClass: null,
    label: null,
    labelClass: null,
    help: null,
    placeholder: null,
    modelValue: null,
    values: null,
    files: null
  },
  setup(e) {
    var T;
    const t = e, l = M(null), { assetsPathResolver: n, fallbackPathResolver: i } = St(), r = {}, d = M(), c = M(((T = t.files) == null ? void 0 : T.map(m)) || []);
    function m(J) {
      return J.filePath = n(J.filePath), J;
    }
    t.values && t.values.length > 0 && (c.value = t.values.map((J) => {
      let j = J.replace(/\\/g, "/");
      return { fileName: wo($t(j, "/"), "."), filePath: j, contentType: Ql(j) };
    }).map(m));
    const h = f(() => t.label ?? Ee(nt(t.id))), y = f(() => t.placeholder ?? h.value);
    let b = qe("ApiState", void 0);
    const p = f(() => ft.call({ responseStatus: t.status ?? (b == null ? void 0 : b.error.value) }, t.id)), v = f(() => [
      "block w-full sm:text-sm rounded-md dark:text-white dark:bg-gray-900 file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-violet-50 dark:file:bg-violet-900 file:text-violet-700 dark:file:text-violet-200 hover:file:bg-violet-100 dark:hover:file:bg-violet-800",
      p.value ? "pr-10 border-red-300 text-red-900 placeholder-red-300 focus:outline-none focus:ring-red-500 focus:border-red-500" : "text-slate-500 dark:text-slate-400",
      t.inputClass
    ]), g = (J) => {
      let j = J.target;
      d.value = "", c.value = Array.from(j.files || []).map((L) => ({
        fileName: L.name,
        filePath: rn(L),
        contentLength: L.size,
        contentType: L.type || Ql(L.name)
      }));
    }, O = () => {
      var J;
      return (J = l.value) == null ? void 0 : J.click();
    }, U = (J) => J == null ? !1 : J.startsWith("data:") || J.startsWith("blob:"), Y = f(() => {
      if (c.value.length > 0)
        return c.value[0].filePath;
      let J = typeof t.modelValue == "string" ? t.modelValue : t.values && t.values[0];
      return J && pt(n(J)) || null;
    }), R = (J) => !J || J.startsWith("data:") || J.endsWith(".svg") ? "" : "rounded-full object-cover";
    function N(J) {
      d.value = i(Y.value);
    }
    return Pt(rs), (J, j) => (a(), u("div", {
      class: w(["flex", e.multiple ? "flex-col" : "justify-between"])
    }, [
      s("div", mc, [
        o(h) ? (a(), u("label", {
          key: 0,
          for: e.id,
          class: w(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, A(o(h)), 11, vc)) : k("", !0),
        s("div", hc, [
          s("span", gc, A(e.help ?? o(h)), 1),
          s("input", Le({
            ref_key: "input",
            ref: l,
            type: "file",
            multiple: e.multiple,
            name: e.id,
            id: e.id,
            class: o(v),
            placeholder: o(y),
            "aria-invalid": o(p) != null,
            "aria-describedby": `${e.id}-error`
          }, J.$attrs, { onChange: g }), null, 16, pc),
          o(p) ? (a(), u("div", yc, wc)) : k("", !0)
        ]),
        o(p) ? (a(), u("p", {
          key: 1,
          class: "mt-2 text-sm text-red-500",
          id: `${e.id}-error`
        }, A(o(p)), 9, xc)) : e.help ? (a(), u("p", {
          key: 2,
          class: "mt-2 text-sm text-gray-500",
          id: `${e.id}-description`
        }, A(e.help), 9, kc)) : k("", !0)
      ]),
      e.multiple ? (a(), u("div", Lc, [
        s("table", Vc, [
          (a(!0), u(Te, null, Ie(c.value, (L) => (a(), u("tr", null, [
            s("td", Sc, [
              s("div", {
                class: "flex w-full",
                title: U(L.filePath) ? "" : L.filePath
              }, [
                s("img", {
                  src: r[o(pt)(L.filePath)] || o(n)(o(pt)(L.filePath)),
                  class: w(["mr-2 h-8 w-8", R(L.filePath)]),
                  onError: (q) => r[o(pt)(L.filePath)] = o(i)(o(pt)(L.filePath))
                }, null, 42, Ac),
                U(L.filePath) ? (a(), u("span", Fc, A(L.fileName), 1)) : (a(), u("a", {
                  key: 0,
                  href: o(n)(L.filePath || ""),
                  target: "_blank",
                  class: "overflow-hidden"
                }, A(L.fileName), 9, Tc))
              ], 8, Mc)
            ]),
            s("td", Ic, [
              L.contentLength && L.contentLength > 0 ? (a(), u("span", Dc, A(o(dn)(L.contentLength)), 1)) : k("", !0)
            ])
          ]))), 256))
        ])
      ])) : (a(), u("div", $c, [
        o(Y) ? (a(), u("div", {
          key: 0,
          class: "shrink-0 cursor-pointer",
          title: U(o(Y)) ? "" : o(Y)
        }, [
          s("img", {
            onClick: O,
            class: w(["h-16 w-16", R(o(Y))]),
            alt: `Current ${o(h) ?? ""}`,
            src: d.value || o(n)(o(Y)),
            onError: N
          }, null, 42, _c)
        ], 8, Cc)) : k("", !0)
      ]))
    ], 2));
  }
}), jc = ["id"], Pc = ["for"], Bc = { class: "relative mt-1" }, Rc = ["id", "placeholder"], Hc = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5 text-gray-400 dark:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M10 3a.75.75 0 01.55.24l3.25 3.5a.75.75 0 11-1.1 1.02L10 4.852 7.3 7.76a.75.75 0 01-1.1-1.02l3.25-3.5A.75.75 0 0110 3zm-3.76 9.2a.75.75 0 011.06.04l2.7 2.908 2.7-2.908a.75.75 0 111.1 1.02l-3.25 3.5a.75.75 0 01-1.1 0l-3.25-3.5a.75.75 0 01.04-1.06z",
    "clip-rule": "evenodd"
  })
], -1), Ec = [
  Hc
], zc = ["id"], Nc = ["onMouseover", "onClick"], Uc = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M16.704 4.153a.75.75 0 01.143 1.052l-8 10.5a.75.75 0 01-1.127.075l-4.5-4.5a.75.75 0 011.06-1.06l3.894 3.893 7.48-9.817a.75.75 0 011.05-.143z",
    "clip-rule": "evenodd"
  })
], -1), qc = [
  Uc
], Qc = {
  key: 2,
  class: "absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none",
  tabindex: "-1"
}, Kc = /* @__PURE__ */ s("svg", {
  class: "h-5 w-5 text-red-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z",
    "clip-rule": "evenodd"
  })
], -1), Zc = [
  Kc
], Gc = ["id"], Wc = ["id"], Jc = /* @__PURE__ */ ue({
  __name: "Autocomplete",
  props: {
    status: null,
    id: null,
    type: null,
    label: null,
    help: null,
    placeholder: null,
    multiple: { type: Boolean, default: !1 },
    required: { type: Boolean },
    options: { default: () => [] },
    modelValue: null,
    match: null,
    viewCount: { default: 100 },
    pageSize: { default: 8 }
  },
  emits: ["update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const n = e, i = M(!1);
    t({ toggle: K });
    function r(S) {
      return Array.isArray(n.modelValue) && n.modelValue.indexOf(S) >= 0;
    }
    const d = f(() => n.label ?? Ee(nt(n.id)));
    let c = qe("ApiState", void 0);
    const m = f(() => ft.call({ responseStatus: n.status ?? (c == null ? void 0 : c.error.value) }, n.id)), h = f(() => [lt.base, m.value ? lt.invalid : lt.valid]), y = M(null), b = M(""), p = M(null), v = M(n.viewCount), g = M([]), O = f(() => b.value ? n.options.filter((le) => n.match(le, b.value)).slice(0, v.value) : n.options), U = ["Tab", "Escape", "ArrowDown", "ArrowUp", "Enter", "PageUp", "PageDown", "Home", "End"];
    function Y(S) {
      p.value = S, g.value.indexOf(S) > Math.floor(v.value * 0.9) && (v.value += n.viewCount, te());
    }
    const R = [",", `
`, "	"];
    function N(S) {
      var x;
      const le = (x = S.clipboardData) == null ? void 0 : x.getData("Text");
      T(le);
    }
    function T(S) {
      if (!S)
        return;
      const le = R.some((x) => S.includes(x));
      if (!n.multiple || !le) {
        const x = n.options.filter((W) => n.match(W, S));
        x.length == 1 && (ee(x[0]), i.value = !1, cl());
      } else if (le) {
        const x = new RegExp("\\r|\\n|\\t|,"), E = S.split(x).filter((G) => G.trim()).map((G) => n.options.find((C) => n.match(C, G))).filter((G) => !!G);
        if (E.length > 0) {
          b.value = "", i.value = !1, p.value = null;
          let G = Array.from(n.modelValue || []);
          E.forEach((C) => {
            r(C) ? G = G.filter((_) => _ != C) : G.push(C);
          }), l("update:modelValue", G), cl();
        }
      }
    }
    function J(S) {
      U.indexOf(S.code) || ne();
    }
    function j(S) {
      if (!(S.shiftKey || S.ctrlKey || S.altKey)) {
        if (!i.value) {
          S.code == "ArrowDown" && (i.value = !0, p.value = g.value[0]);
          return;
        }
        if (S.code == "Escape")
          i.value && (S.stopPropagation(), i.value = !1);
        else if (S.code == "Tab")
          i.value = !1;
        else if (S.code == "Home")
          p.value = g.value[0], q();
        else if (S.code == "End")
          p.value = g.value[g.value.length - 1], q();
        else if (S.code == "ArrowDown") {
          if (!p.value)
            p.value = g.value[0];
          else {
            const le = g.value.indexOf(p.value);
            p.value = le + 1 < g.value.length ? g.value[le + 1] : g.value[0];
          }
          D();
        } else if (S.code == "ArrowUp") {
          if (!p.value)
            p.value = g.value[g.value.length - 1];
          else {
            const le = g.value.indexOf(p.value);
            p.value = le - 1 >= 0 ? g.value[le - 1] : g.value[g.value.length - 1];
          }
          D();
        } else
          S.code == "Enter" && (p.value ? (ee(p.value), n.multiple || (S.preventDefault(), cl())) : i.value = !1);
      }
    }
    const L = { behavior: "smooth", block: "nearest", inline: "nearest", scrollMode: "if-needed" };
    function q() {
      setTimeout(() => {
        let S = pl(`#${n.id}-autocomplete li.active`);
        S && S.scrollIntoView(L);
      }, 0);
    }
    function D() {
      setTimeout(() => {
        let S = pl(`#${n.id}-autocomplete li.active`);
        S && ("scrollIntoViewIfNeeded" in S ? S.scrollIntoViewIfNeeded(L) : S.scrollIntoView(L));
      }, 0);
    }
    function K(S) {
      var le;
      i.value = S, S && (ne(), (le = y.value) == null || le.focus());
    }
    function ne() {
      i.value = !0, te();
    }
    function ee(S) {
      if (b.value = "", i.value = !1, n.multiple) {
        let le = Array.from(n.modelValue || []);
        r(S) ? le = le.filter((x) => x != S) : le.push(S), p.value = null, l("update:modelValue", le);
      } else {
        let le = S;
        n.modelValue == S && (le = null), l("update:modelValue", le);
      }
    }
    function te() {
      g.value = O.value;
    }
    return Lt(b, te), (S, le) => (a(), u("div", {
      id: `${e.id}-autocomplete`
    }, [
      o(d) ? (a(), u("label", {
        key: 0,
        for: `${e.id}-text`,
        class: "block text-sm font-medium text-gray-700 dark:text-gray-300"
      }, A(o(d)), 9, Pc)) : k("", !0),
      s("div", Bc, [
        kt(s("input", Le({
          ref_key: "txtInput",
          ref: y,
          id: `${e.id}-text`,
          type: "text",
          role: "combobox",
          "aria-controls": "options",
          "aria-expanded": "false",
          autocomplete: "off",
          spellcheck: "false",
          "onUpdate:modelValue": le[0] || (le[0] = (x) => b.value = x),
          class: o(h),
          placeholder: e.multiple || !e.modelValue ? e.placeholder : "",
          onFocus: ne,
          onKeydown: j,
          onKeyup: J,
          onClick: ne,
          onPaste: N,
          required: !1
        }, S.$attrs), null, 16, Rc), [
          [so, b.value]
        ]),
        s("button", {
          type: "button",
          onClick: le[1] || (le[1] = (x) => K(!i.value)),
          class: "absolute inset-y-0 right-0 flex items-center rounded-r-md px-2 focus:outline-none",
          tabindex: "-1"
        }, Ec),
        i.value ? (a(), u("ul", {
          key: 0,
          class: "absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white dark:bg-black py-1 text-base shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm",
          onKeydown: j,
          id: `${e.id}-options`,
          role: "listbox"
        }, [
          (a(!0), u(Te, null, Ie(g.value, (x) => (a(), u("li", {
            class: w([x === p.value ? "active bg-indigo-600 text-white" : "text-gray-900 dark:text-gray-100", "relative cursor-default select-none py-2 pl-3 pr-9"]),
            onMouseover: (W) => Y(x),
            onClick: (W) => ee(x),
            role: "option",
            tabindex: "-1"
          }, [
            Z(S.$slots, "item", It(gl(x))),
            r(x) ? (a(), u("span", {
              key: 0,
              class: w(["absolute inset-y-0 right-0 flex items-center pr-4", x === p.value ? "text-white" : "text-indigo-600"])
            }, qc, 2)) : k("", !0)
          ], 42, Nc))), 256))
        ], 40, zc)) : !e.multiple && e.modelValue ? (a(), u("div", {
          key: 1,
          onKeydown: j,
          onClick: le[2] || (le[2] = (x) => K(!i.value)),
          class: "h-8 -mt-8 ml-3 pt-0.5"
        }, [
          Z(S.$slots, "item", It(gl(e.modelValue)))
        ], 32)) : k("", !0),
        o(m) ? (a(), u("div", Qc, Zc)) : k("", !0)
      ]),
      o(m) ? (a(), u("p", {
        key: 1,
        class: "mt-2 text-sm text-red-500",
        id: `${e.id}-error`
      }, A(o(m)), 9, Gc)) : e.help ? (a(), u("p", {
        key: 2,
        class: "mt-2 text-sm text-gray-500",
        id: `${e.id}-description`
      }, A(e.help), 9, Wc)) : k("", !0)
    ], 8, jc));
  }
}), Xc = ["id", "name", "value"], Yc = { class: "block truncate" }, e0 = /* @__PURE__ */ ue({
  __name: "Combobox",
  props: {
    id: null,
    modelValue: null,
    multiple: { type: Boolean },
    options: null,
    values: null,
    entries: null
  },
  emits: ["update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const n = e;
    t({
      toggle(p) {
        var v;
        (v = d.value) == null || v.toggle(p);
      }
    });
    function i(p) {
      l("update:modelValue", p);
    }
    const r = f(() => n.multiple != null ? n.multiple : Array.isArray(n.modelValue)), d = M();
    function c(p, v) {
      return !v || p.value.toLowerCase().includes(v.toLowerCase());
    }
    const m = f(() => n.entries || (n.values ? n.values.map((p) => ({ key: p, value: p })) : n.options ? Object.keys(n.options).map((p) => ({ key: p, value: n.options[p] })) : [])), h = M(r.value ? [] : null);
    function y() {
      let p = n.modelValue && typeof n.modelValue == "object" ? n.modelValue.key : n.modelValue;
      p == null || p === "" ? h.value = r.value ? [] : null : typeof p == "string" ? h.value = m.value.find((v) => v.key === p) || null : Array.isArray(p) && (h.value = m.value.filter((v) => p.includes(v.key)));
    }
    Xe(y);
    const b = f(() => h.value == null ? "" : Array.isArray(h.value) ? h.value.map((p) => encodeURIComponent(p.key)).join(",") : h.value.key);
    return (p, v) => {
      const g = X("Autocomplete");
      return a(), u(Te, null, [
        s("input", {
          type: "hidden",
          id: e.id,
          name: e.id,
          value: o(b)
        }, null, 8, Xc),
        $e(g, Le({
          ref_key: "input",
          ref: d,
          id: e.id,
          options: o(m),
          match: c,
          multiple: o(r)
        }, p.$attrs, {
          modelValue: h.value,
          "onUpdate:modelValue": [
            v[0] || (v[0] = (O) => h.value = O),
            i
          ]
        }), {
          item: ke(({ key: O, value: U }) => [
            s("span", Yc, A(U), 1)
          ]),
          _: 1
        }, 16, ["id", "options", "multiple", "modelValue"])
      ], 64);
    };
  }
}), t0 = /* @__PURE__ */ ue({
  __name: "DynamicInput",
  props: {
    input: null,
    modelValue: null,
    api: null
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, n = f(() => l.input.type || "text"), i = "ignore,css,options,meta,allowableValues,allowableEntries,op,prop,type,id,name".split(","), r = f(() => ut(l.input, i)), d = M(Ue(
      l.modelValue[l.input.id],
      (m) => l.input.type === "file" ? null : l.input.type === "date" && m instanceof Date ? $l(m) : l.input.type === "time" ? Xn(m) : m
    ));
    Lt(d, () => {
      l.modelValue[l.input.id] = d.value, t("update:modelValue", l.modelValue);
    });
    const c = f(() => {
      const m = l.modelValue[l.input.id];
      if (l.input.type !== "file" || !m)
        return [];
      if (typeof m == "string")
        return [{ filePath: m, fileName: $t(m, "/") }];
      if (!Array.isArray(m) && typeof m == "object")
        return m;
      if (Array.isArray(m)) {
        const h = [];
        return m.forEach((y) => {
          typeof y == "string" ? h.push({ filePath: y, fileName: $t(y, "/") }) : typeof y == "object" && h.push(y);
        }), h;
      }
    });
    return (m, h) => {
      var R, N, T, J, j, L, q, D, K, ne, ee, te, S, le, x, W, E, G, C, _, B, de, F, V, re, ye, fe, H;
      const y = X("SelectInput"), b = X("CheckboxInput"), p = X("TagInput"), v = X("Combobox"), g = X("FileInput"), O = X("TextareaInput"), U = X("MarkdownInput"), Y = X("TextInput");
      return o(ie).component(o(n)) ? (a(), se(Kn(o(ie).component(o(n))), Le({
        key: 0,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[0] || (h[0] = (P) => d.value = P),
        status: (R = e.api) == null ? void 0 : R.error,
        "input-class": (N = e.input.css) == null ? void 0 : N.input,
        "label-class": (T = e.input.css) == null ? void 0 : T.label
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o(n) == "select" ? (a(), se(y, Le({
        key: 1,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[1] || (h[1] = (P) => d.value = P),
        status: (J = e.api) == null ? void 0 : J.error,
        "input-class": (j = e.input.css) == null ? void 0 : j.input,
        "label-class": (L = e.input.css) == null ? void 0 : L.label,
        entries: e.input.allowableEntries,
        values: e.input.allowableValues
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "entries", "values"])) : o(n) == "checkbox" ? (a(), se(b, Le({
        key: 2,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[2] || (h[2] = (P) => d.value = P),
        status: (q = e.api) == null ? void 0 : q.error,
        "input-class": (D = e.input.css) == null ? void 0 : D.input,
        "label-class": (K = e.input.css) == null ? void 0 : K.label
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o(n) == "tag" ? (a(), se(p, Le({
        key: 3,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[3] || (h[3] = (P) => d.value = P),
        status: (ne = e.api) == null ? void 0 : ne.error,
        "input-class": (ee = e.input.css) == null ? void 0 : ee.input,
        "label-class": (te = e.input.css) == null ? void 0 : te.label,
        allowableValues: e.input.allowableValues,
        string: ((S = e.input.prop) == null ? void 0 : S.type) == "String"
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "allowableValues", "string"])) : o(n) == "combobox" ? (a(), se(v, Le({
        key: 4,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[4] || (h[4] = (P) => d.value = P),
        status: (le = e.api) == null ? void 0 : le.error,
        "input-class": (x = e.input.css) == null ? void 0 : x.input,
        "label-class": (W = e.input.css) == null ? void 0 : W.label,
        entries: e.input.allowableEntries,
        values: e.input.allowableValues
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class", "entries", "values"])) : o(n) == "file" ? (a(), se(g, Le({
        key: 5,
        id: e.input.id,
        status: (E = e.api) == null ? void 0 : E.error,
        modelValue: d.value,
        "onUpdate:modelValue": h[5] || (h[5] = (P) => d.value = P),
        "input-class": (G = e.input.css) == null ? void 0 : G.input,
        "label-class": (C = e.input.css) == null ? void 0 : C.label,
        files: o(c)
      }, o(r)), null, 16, ["id", "status", "modelValue", "input-class", "label-class", "files"])) : o(n) == "textarea" ? (a(), se(O, Le({
        key: 6,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[6] || (h[6] = (P) => d.value = P),
        status: (_ = e.api) == null ? void 0 : _.error,
        "input-class": (B = e.input.css) == null ? void 0 : B.input,
        "label-class": (de = e.input.css) == null ? void 0 : de.label
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : o(n) == "MarkdownInput" ? (a(), se(U, Le({
        key: 7,
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[7] || (h[7] = (P) => d.value = P),
        status: (F = e.api) == null ? void 0 : F.error,
        "input-class": (V = e.input.css) == null ? void 0 : V.input,
        "label-class": (re = e.input.css) == null ? void 0 : re.label
      }, o(r)), null, 16, ["id", "modelValue", "status", "input-class", "label-class"])) : (a(), se(Y, Le({
        key: 8,
        type: o(n),
        id: e.input.id,
        modelValue: d.value,
        "onUpdate:modelValue": h[8] || (h[8] = (P) => d.value = P),
        status: (ye = e.api) == null ? void 0 : ye.error,
        "input-class": (fe = e.input.css) == null ? void 0 : fe.input,
        "label-class": (H = e.input.css) == null ? void 0 : H.label
      }, o(r)), null, 16, ["type", "id", "modelValue", "status", "input-class", "label-class"]));
    };
  }
}), l0 = { class: "lookup-field" }, n0 = ["name", "value"], s0 = {
  key: 0,
  class: "flex justify-between"
}, o0 = ["for"], a0 = {
  key: 0,
  class: "flex items-center"
}, i0 = { class: "text-sm text-gray-500 dark:text-gray-400 pr-1" }, r0 = /* @__PURE__ */ s("span", { class: "sr-only" }, "Clear", -1), u0 = /* @__PURE__ */ s("svg", {
  class: "h-4 w-4",
  xmlns: "http://www.w3.org/2000/svg",
  fill: "none",
  viewBox: "0 0 24 24",
  "stroke-width": "1.5",
  stroke: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-linejoin": "round",
    d: "M6 18L18 6M6 6l12 12"
  })
], -1), d0 = [
  r0,
  u0
], c0 = {
  key: 1,
  class: "mt-1 relative"
}, f0 = { class: "w-full inline-flex truncate" }, m0 = { class: "text-blue-700 dark:text-blue-300 flex cursor-pointer" }, v0 = /* @__PURE__ */ s("span", { class: "absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none" }, [
  /* @__PURE__ */ s("svg", {
    class: "h-5 w-5 text-gray-400 dark:text-gray-500",
    xmlns: "http://www.w3.org/2000/svg",
    viewBox: "0 0 20 20",
    fill: "currentColor",
    "aria-hidden": "true"
  }, [
    /* @__PURE__ */ s("path", {
      "fill-rule": "evenodd",
      d: "M10 3a1 1 0 01.707.293l3 3a1 1 0 01-1.414 1.414L10 5.414 7.707 7.707a1 1 0 01-1.414-1.414l3-3A1 1 0 0110 3zm-3.707 9.293a1 1 0 011.414 0L10 14.586l2.293-2.293a1 1 0 011.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z",
      "clip-rule": "evenodd"
    })
  ])
], -1), h0 = ["id"], g0 = ["id"], p0 = /* @__PURE__ */ ue({
  __name: "LookupInput",
  props: {
    id: null,
    status: null,
    input: null,
    metadataType: null,
    modelValue: null,
    label: null,
    labelClass: null,
    help: null
  },
  emits: ["update:modelValue"],
  setup(e, { emit: t }) {
    const l = e, { config: n } = St(), { metadataApi: i } = ot(), r = f(() => l.id || l.input.id), d = f(() => l.label ?? Ee(nt(r.value)));
    let c = qe("ApiState", void 0);
    const m = qe("client"), h = f(() => ft.call({ responseStatus: l.status ?? (c == null ? void 0 : c.error.value) }, r.value)), y = M(""), b = M(""), p = f(() => be(l.modelValue, r.value)), v = f(() => Je(l.metadataType).find((R) => R.name.toLowerCase() == r.value.toLowerCase())), g = f(() => {
      var R, N, T;
      return ((T = st((N = (R = v.value) == null ? void 0 : R.ref) == null ? void 0 : N.model)) == null ? void 0 : T.icon) || n.value.tableIcon;
    });
    let O;
    function U(R) {
      if (R) {
        if (O == null) {
          console.warn("No ModalProvider required by LookupInput");
          return;
        }
        O.openModal({ name: "ModalLookup", ref: R }, (N) => {
          if (console.debug("openModal", y.value, " -> ", N, wt.setRefValue(R, N), R), N) {
            const T = be(N, R.refId);
            y.value = wt.setRefValue(R, N) || T;
            const J = o(l.modelValue);
            J[r.value] = T, t("update:modelValue", J);
          }
        });
      }
    }
    function Y() {
      l.modelValue[r.value] = null, y.value = "";
    }
    return Xe(async () => {
      var q, D;
      O = qe("ModalProvider", void 0);
      const R = l.modelValue;
      l.modelValue[r.value] || (l.modelValue[r.value] = null);
      const N = v.value, T = N == null ? void 0 : N.ref;
      if (!T) {
        console.warn(`No RefInfo for property '${r.value}'`);
        return;
      }
      y.value = "";
      let J = T.selfId == null ? be(R, N.name) : be(R, T.selfId);
      if (Zt(J) && (J = be(R, T.refId)), J == null)
        return;
      if (((q = i.value) == null ? void 0 : q.operations.find((K) => {
        var ne;
        return ((ne = K.dataModel) == null ? void 0 : ne.name) == T.model;
      })) != null) {
        const K = be(R, N.name);
        if (Zt(K))
          return;
        if (y.value = `${K}`, b.value = N.name, T.refLabel != null) {
          const ne = Je(l.metadataType).find((te) => te.type == T.model);
          ne == null && console.warn(`Could not find ${T.model} Property on ${l.metadataType.name}`);
          const ee = ne != null ? be(R, ne.name) : null;
          if (ee != null) {
            let te = be(ee, T.refLabel);
            te && (y.value = `${te}`, wt.setValue(T.model, J, T.refLabel, te));
          } else {
            const te = ((D = N.attributes) == null ? void 0 : D.some((le) => le.name == "Computed")) == !0;
            let S = await wt.getOrFetchValue(m, i.value, T.model, T.refId, T.refLabel, te, J);
            y.value = S || `${T.model}: ${y.value}`;
          }
        }
      }
    }), (R, N) => {
      var J;
      const T = X("Icon");
      return a(), u("div", l0, [
        s("input", {
          type: "hidden",
          name: o(r),
          value: o(p)
        }, null, 8, n0),
        o(d) ? (a(), u("div", s0, [
          s("label", {
            for: o(r),
            class: w(`block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
          }, A(o(d)), 11, o0),
          o(p) ? (a(), u("div", a0, [
            s("span", i0, A(o(p)), 1),
            s("button", {
              onClick: Y,
              type: "button",
              title: "clear",
              class: "mr-1 rounded-md text-gray-400 dark:text-gray-500 hover:text-gray-500 dark:hover:text-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:ring-offset-black"
            }, d0)
          ])) : k("", !0)
        ])) : k("", !0),
        (J = o(v)) != null && J.ref ? (a(), u("div", c0, [
          s("button", {
            type: "button",
            class: "lookup flex relative w-full bg-white dark:bg-black border border-gray-300 dark:border-gray-700 rounded-md shadow-sm pl-3 pr-10 py-2 text-left focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm",
            onClick: N[0] || (N[0] = (j) => U(o(v).ref)),
            "aria-haspopup": "listbox",
            "aria-expanded": "true",
            "aria-labelledby": "listbox-label"
          }, [
            s("span", f0, [
              s("span", m0, [
                $e(T, {
                  class: "mr-1 w-5 h-5",
                  image: o(g)
                }, null, 8, ["image"]),
                s("span", null, A(y.value), 1)
              ])
            ]),
            v0
          ])
        ])) : k("", !0),
        o(h) ? (a(), u("p", {
          key: 2,
          class: "mt-2 text-sm text-red-500",
          id: `${o(r)}-error`
        }, A(o(h)), 9, h0)) : e.help ? (a(), u("p", {
          key: 3,
          class: "mt-2 text-sm text-gray-500",
          id: `${o(r)}-description`
        }, A(e.help), 9, g0)) : k("", !0)
      ]);
    };
  }
}), y0 = /* @__PURE__ */ ue({
  __name: "AutoFormFields",
  props: {
    modelValue: null,
    type: null,
    api: null,
    formLayout: null,
    configureField: null,
    configureFormLayout: null,
    hideSummary: { type: Boolean },
    flexClass: { default: "flex flex-1 flex-col justify-between" },
    divideClass: { default: "divide-y divide-gray-200 px-4 sm:px-6" },
    spaceClass: { default: "space-y-6 pt-6 pb-5" },
    fieldsetClass: { default: "grid grid-cols-12 gap-6" }
  },
  emits: ["update:modelValue"],
  setup(e, { expose: t, emit: l }) {
    const n = e;
    t({ forceUpdate: i, props: n, updateValue: d });
    function i() {
      var N;
      const R = Be();
      (N = R == null ? void 0 : R.proxy) == null || N.$forceUpdate();
    }
    function r(R, N) {
      d(R.id, be(N, R.id));
    }
    function d(R, N) {
      n.modelValue[R] = N, l("update:modelValue", n.modelValue), i();
    }
    const { metadataApi: c, apiOf: m, typeOf: h, typeOfRef: y, createFormLayout: b, Crud: p } = ot(), v = f(() => n.type || Bt(n.modelValue)), g = f(() => h(v.value)), O = f(() => {
      var R, N;
      return y((N = (R = c.value) == null ? void 0 : R.operations.find((T) => T.request.name == v.value)) == null ? void 0 : N.dataModel) || g.value;
    }), U = f(() => {
      const R = g.value;
      if (!R) {
        if (n.formLayout) {
          const q = n.formLayout.map((D) => {
            const K = { name: D.id, type: va(D.type) }, ne = Object.assign({ prop: K }, D);
            return n.configureField && n.configureField(ne), ne;
          });
          return n.configureFormLayout && n.configureFormLayout(q), q;
        }
        throw new Error(`MetadataType for ${v.value} not found`);
      }
      const N = Je(R), T = O.value, J = n.formLayout ? n.formLayout : b(R), j = [], L = m(R.name);
      return J.forEach((q) => {
        var ee;
        const D = N.find((te) => te.name == q.name);
        if (q.ignore)
          return;
        const K = ((ee = T == null ? void 0 : T.properties) == null ? void 0 : ee.find((te) => {
          var S;
          return te.name.toLowerCase() == ((S = q.name) == null ? void 0 : S.toLowerCase());
        })) ?? D, ne = Object.assign({ prop: K, op: L }, q);
        n.configureField && n.configureField(ne), j.push(ne);
      }), n.configureFormLayout && n.configureFormLayout(j), j;
    }), Y = f(() => U.value.filter((R) => R.type != "hidden").map((R) => R.id));
    return (R, N) => {
      var L;
      const T = X("ErrorSummary"), J = X("LookupInput"), j = X("DynamicInput");
      return a(), u(Te, null, [
        e.hideSummary ? k("", !0) : (a(), se(T, {
          key: 0,
          status: (L = e.api) == null ? void 0 : L.error,
          except: o(Y)
        }, null, 8, ["status", "except"])),
        s("div", {
          class: w(e.flexClass)
        }, [
          s("div", {
            class: w(e.divideClass)
          }, [
            s("div", {
              class: w(e.spaceClass)
            }, [
              s("fieldset", {
                class: w(e.fieldsetClass)
              }, [
                (a(!0), u(Te, null, Ie(o(U), (q) => {
                  var D, K, ne;
                  return a(), u("div", {
                    key: q.id,
                    class: w([
                      "w-full",
                      ((D = q.css) == null ? void 0 : D.field) ?? (q.type == "textarea" ? "col-span-12" : "col-span-12 xl:col-span-6" + (q.type == "checkbox" ? " flex items-center" : "")),
                      q.type == "hidden" ? "hidden" : ""
                    ])
                  }, [
                    ((K = q.prop) == null ? void 0 : K.ref) != null && q.type != "file" && !q.prop.isPrimaryKey ? (a(), se(J, {
                      key: 0,
                      metadataType: o(O),
                      input: q,
                      modelValue: e.modelValue,
                      "onUpdate:modelValue": (ee) => r(q, ee),
                      status: (ne = e.api) == null ? void 0 : ne.error
                    }, null, 8, ["metadataType", "input", "modelValue", "onUpdate:modelValue", "status"])) : (a(), se(j, {
                      key: 1,
                      input: q,
                      modelValue: e.modelValue,
                      "onUpdate:modelValue": N[0] || (N[0] = (ee) => R.$emit("update:modelValue", ee)),
                      api: e.api
                    }, null, 8, ["input", "modelValue", "api"]))
                  ], 2);
                }), 128))
              ], 2)
            ], 2)
          ], 2)
        ], 2)
      ], 64);
    };
  }
});
function Tl() {
  const e = M(!1), t = M(), l = M(), n = qe("client");
  function i({ message: v, errorCode: g, fieldName: O, errors: U }) {
    return g || (g = "Exception"), U || (U = []), t.value = O ? new Pl({
      errorCode: g,
      message: v,
      errors: [new Hn({ fieldName: O, errorCode: g, message: v })]
    }) : new Pl({ errorCode: g, message: v, errors: U });
  }
  function r({ fieldName: v, message: g, errorCode: O }) {
    if (O || (O = "Exception"), !t.value)
      i({ fieldName: v, message: g, errorCode: O });
    else {
      let U = new Pl(t.value);
      U.errors = [
        ...(U.errors || []).filter((Y) => {
          var R;
          return ((R = Y.fieldName) == null ? void 0 : R.toLowerCase()) !== (v == null ? void 0 : v.toLowerCase());
        }),
        new Hn({ fieldName: v, message: g, errorCode: O })
      ], t.value = U;
    }
  }
  async function d(v, g, O) {
    e.value = !0;
    let U = await n.api(At(v), g, O);
    return e.value = !1, l.value = U.response, t.value = U.error, U;
  }
  async function c(v, g, O) {
    e.value = !0;
    let U = await n.apiVoid(At(v), g, O);
    return e.value = !1, l.value = U.response, t.value = U.error, U;
  }
  async function m(v, g, O, U) {
    e.value = !0;
    let Y = await n.apiForm(At(v), g, O, U);
    return e.value = !1, l.value = Y.response, t.value = Y.error, Y;
  }
  async function h(v, g, O, U) {
    e.value = !0;
    let Y = await n.apiFormVoid(At(v), g, O, U);
    return e.value = !1, l.value = Y.response, t.value = Y.error, Y;
  }
  async function y(v, g, O, U) {
    return ts(n, v, g, O, U);
  }
  function b(v, g) {
    const O = M(new We()), U = ls(async (Y) => {
      O.value = await n.api(Y);
    }, g == null ? void 0 : g.delayMs);
    return hl(async () => {
      const Y = v(), R = sn(_l(Y));
      R && (O.value = new We({ response: R })), (g == null ? void 0 : g.delayMs) === 0 ? O.value = await n.api(Y) : U(Y);
    }), (async () => O.value = await n.api(v(), g == null ? void 0 : g.args, g == null ? void 0 : g.method))(), O;
  }
  let p = { setError: i, addFieldError: r, loading: e, error: t, api: d, apiVoid: c, apiForm: m, apiFormVoid: h, swr: y, swrEffect: b, unRefs: At, setRef: Yn };
  return Xt("ApiState", p), p;
}
const b0 = { key: 0 }, w0 = { class: "text-red-700" }, x0 = /* @__PURE__ */ s("b", null, "type", -1), k0 = { key: 0 }, $0 = { key: 2 }, C0 = ["innerHTML"], _0 = /* @__PURE__ */ s("input", {
  type: "submit",
  class: "hidden"
}, null, -1), L0 = { class: "flex justify-end" }, V0 = /* @__PURE__ */ s("div", null, null, -1), S0 = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, M0 = /* @__PURE__ */ s("div", { class: "fixed inset-0" }, null, -1), A0 = { class: "fixed inset-0 overflow-hidden" }, T0 = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, F0 = { class: "flex-1" }, I0 = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, D0 = { class: "flex items-start justify-between space-x-3" }, O0 = { class: "space-y-1" }, j0 = { key: 0 }, P0 = { key: 2 }, B0 = ["innerHTML"], R0 = { class: "flex h-7 items-center" }, H0 = { class: "flex justify-end" }, E0 = /* @__PURE__ */ ue({
  __name: "AutoForm",
  props: {
    type: null,
    modelValue: null,
    heading: null,
    subHeading: null,
    showLoading: { type: Boolean, default: !0 },
    jsconfig: { default: "eccn,edv" },
    formStyle: { default: "card" },
    configureField: null,
    configureFormLayout: null,
    panelClass: null,
    bodyClass: null,
    formClass: null,
    innerFormClass: null,
    headerClass: { default: "p-6" },
    buttonsClass: null,
    headingClass: null,
    subHeadingClass: null,
    submitLabel: { default: "Submit" },
    allowSubmit: null
  },
  emits: ["success", "error", "update:modelValue", "done"],
  setup(e, { expose: t, emit: l }) {
    const n = e, i = M(), r = M(1), d = M();
    t({ forceUpdate: c, props: n, setModel: m, formFields: i, submit: W, close: de });
    function c() {
      var re;
      r.value++, ee.value = ne();
      const V = Be();
      (re = V == null ? void 0 : V.proxy) == null || re.$forceUpdate();
    }
    async function m(V) {
      Object.assign(ee.value, V), c(), await xt(() => null);
    }
    Xt("ModalProvider", {
      openModal: p
    });
    const y = M(), b = M();
    function p(V, re) {
      y.value = V, b.value = re;
    }
    async function v(V) {
      b.value && b.value(V), y.value = void 0, b.value = void 0;
    }
    const g = Tl(), { getTypeName: O } = ns(), { typeOf: U, Crud: Y, createDto: R } = ot(), N = M(new We()), T = f(() => n.panelClass || Ze.panelClass(n.formStyle)), J = f(() => n.formClass || n.formStyle == "card" ? "shadow sm:rounded-md" : Tt.formClass), j = f(() => n.headingClass || Ze.headingClass(n.formStyle)), L = f(() => n.subHeadingClass || Ze.subHeadingClass(n.formStyle)), q = f(() => typeof n.buttonsClass == "string" ? n.buttonsClass : Ze.buttonsClass), D = f(() => {
      var V;
      return n.type ? O(n.type) : (V = n.modelValue) != null && V.getTypeName ? n.modelValue.getTypeName() : null;
    }), K = f(() => U(D.value)), ne = () => n.modelValue || le(), ee = M(ne()), te = f(() => g.loading.value), S = f(() => {
      var V;
      return n.heading || ((V = U(D.value)) == null ? void 0 : V.description) || Ee(D.value);
    });
    function le() {
      return typeof n.type == "string" ? R(n.type) : n.type ? new n.type() : n.modelValue;
    }
    async function x(V) {
      if (!V || V.tagName != "FORM") {
        console.error("Not a valid form", V);
        return;
      }
      const re = le();
      let ye = Ue(re == null ? void 0 : re.getMethod, (P) => typeof P == "function" ? P() : null) || "POST", fe = Ue(re == null ? void 0 : re.createResponse, (P) => typeof P == "function" ? P() : null) == null;
      const H = n.jsconfig;
      if (tn.hasRequestBody(ye)) {
        let P = new re.constructor(), me = new FormData(V);
        console.debug("AutoForm.submitForm", P, me), fe ? N.value = await g.apiFormVoid(P, me, { jsconfig: H }) : N.value = await g.apiForm(P, me, { jsconfig: H });
      } else {
        let P = new re.constructor(ee.value);
        console.debug("AutoForm.submit", P), fe ? N.value = await g.apiVoid(P, { jsconfig: H }) : N.value = await g.api(P, { jsconfig: H });
      }
      N.value.succeeded ? (l("success", N.value.response), de()) : l("error", N.value.error);
    }
    async function W() {
      x(d.value);
    }
    function E(V) {
      l("update:modelValue", V);
    }
    function G() {
      l("done");
    }
    const C = M(!1), _ = M(""), B = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    Lt(C, () => {
      Ct(B, _, C.value), C.value || setTimeout(G, 700);
    }), C.value = !0;
    function de() {
      n.formStyle == "slideOver" ? C.value = !1 : G();
    }
    const F = (V) => {
      V.key === "Escape" && de();
    };
    return Xe(() => window.addEventListener("keydown", F)), Pt(() => window.removeEventListener("keydown", F)), (V, re) => {
      var ve, Ve, Fe, z, Q, oe, ge, Se, Oe, Re, Me;
      const ye = X("AutoFormFields"), fe = X("FormLoading"), H = X("PrimaryButton"), P = X("CloseButton"), me = X("SecondaryButton"), Ce = X("ModalLookup");
      return a(), u("div", null, [
        o(K) ? e.formStyle == "card" ? (a(), u("div", {
          key: 1,
          class: w(o(T))
        }, [
          s("form", {
            ref_key: "elForm",
            ref: d,
            onSubmit: re[0] || (re[0] = Ne((De) => x(De.target), ["prevent"])),
            autocomplete: "off",
            class: w(e.innerFormClass)
          }, [
            s("div", {
              class: w(e.bodyClass)
            }, [
              s("div", {
                class: w(e.headerClass)
              }, [
                V.$slots.heading ? (a(), u("div", k0, [
                  Z(V.$slots, "heading")
                ])) : (a(), u("h3", {
                  key: 1,
                  class: w(o(j))
                }, A(o(S)), 3)),
                V.$slots.subheading ? (a(), u("div", $0, [
                  Z(V.$slots, "subheading")
                ])) : e.subHeading ? (a(), u("p", {
                  key: 3,
                  class: w(o(L))
                }, A(e.subHeading), 3)) : (ve = o(K)) != null && ve.notes ? (a(), u("p", {
                  key: 4,
                  class: w(["notes", o(L)]),
                  innerHTML: (Ve = o(K)) == null ? void 0 : Ve.notes
                }, null, 10, C0)) : k("", !0)
              ], 2),
              Z(V.$slots, "header", {
                instance: (Fe = Be()) == null ? void 0 : Fe.exposed,
                model: ee.value
              }),
              _0,
              (a(), se(ye, {
                ref_key: "formFields",
                ref: i,
                key: r.value,
                type: e.type,
                modelValue: ee.value,
                "onUpdate:modelValue": E,
                api: N.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["type", "modelValue", "api", "configureField", "configureFormLayout"])),
              Z(V.$slots, "footer", {
                instance: (z = Be()) == null ? void 0 : z.exposed,
                model: ee.value
              })
            ], 2),
            Z(V.$slots, "buttons", {}, () => {
              var De, Qe;
              return [
                s("div", {
                  class: w(o(q))
                }, [
                  s("div", null, [
                    Z(V.$slots, "leftbuttons", {
                      instance: (De = Be()) == null ? void 0 : De.exposed,
                      model: ee.value
                    })
                  ]),
                  s("div", null, [
                    e.showLoading && o(te) ? (a(), se(fe, { key: 0 })) : k("", !0)
                  ]),
                  s("div", L0, [
                    V0,
                    $e(H, {
                      disabled: e.allowSubmit ? !e.allowSubmit(ee.value) : !1
                    }, {
                      default: ke(() => [
                        xe(A(e.submitLabel), 1)
                      ]),
                      _: 1
                    }, 8, ["disabled"]),
                    Z(V.$slots, "rightbuttons", {
                      instance: (Qe = Be()) == null ? void 0 : Qe.exposed,
                      model: ee.value
                    })
                  ])
                ], 2)
              ];
            })
          ], 34)
        ], 2)) : (a(), u("div", S0, [
          M0,
          s("div", A0, [
            s("div", {
              onMousedown: de,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: re[2] || (re[2] = Ne(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: w(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", _.value])
                }, [
                  s("form", {
                    ref_key: "elForm",
                    ref: d,
                    class: w(o(J)),
                    onSubmit: re[1] || (re[1] = Ne((De) => x(De.target), ["prevent"]))
                  }, [
                    s("div", T0, [
                      s("div", F0, [
                        s("div", I0, [
                          s("div", D0, [
                            s("div", O0, [
                              V.$slots.heading ? (a(), u("div", j0, [
                                Z(V.$slots, "heading")
                              ])) : (a(), u("h3", {
                                key: 1,
                                class: w(o(j))
                              }, A(o(S)), 3)),
                              V.$slots.subheading ? (a(), u("div", P0, [
                                Z(V.$slots, "subheading")
                              ])) : e.subHeading ? (a(), u("p", {
                                key: 3,
                                class: w(o(L))
                              }, A(e.subHeading), 3)) : (Q = o(K)) != null && Q.notes ? (a(), u("p", {
                                key: 4,
                                class: w(["notes", o(L)]),
                                innerHTML: (oe = o(K)) == null ? void 0 : oe.notes
                              }, null, 10, B0)) : k("", !0)
                            ]),
                            s("div", R0, [
                              $e(P, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: de
                              })
                            ])
                          ])
                        ]),
                        Z(V.$slots, "header", {
                          instance: (ge = Be()) == null ? void 0 : ge.exposed,
                          model: ee.value
                        }),
                        (a(), se(ye, {
                          ref_key: "formFields",
                          ref: i,
                          key: r.value,
                          type: e.type,
                          modelValue: ee.value,
                          "onUpdate:modelValue": E,
                          api: N.value,
                          configureField: e.configureField,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["type", "modelValue", "api", "configureField", "configureFormLayout"])),
                        Z(V.$slots, "footer", {
                          instance: (Se = Be()) == null ? void 0 : Se.exposed,
                          model: ee.value
                        })
                      ])
                    ]),
                    s("div", {
                      class: w(o(q))
                    }, [
                      s("div", null, [
                        Z(V.$slots, "leftbuttons", {
                          instance: (Oe = Be()) == null ? void 0 : Oe.exposed,
                          model: ee.value
                        })
                      ]),
                      s("div", null, [
                        e.showLoading && o(te) ? (a(), se(fe, { key: 0 })) : k("", !0)
                      ]),
                      s("div", H0, [
                        $e(me, {
                          onClick: de,
                          disabled: o(te)
                        }, {
                          default: ke(() => [
                            xe("Cancel")
                          ]),
                          _: 1
                        }, 8, ["disabled"]),
                        $e(H, {
                          class: "ml-4",
                          disabled: e.allowSubmit ? !e.allowSubmit(ee.value) : !1
                        }, {
                          default: ke(() => [
                            xe(A(e.submitLabel), 1)
                          ]),
                          _: 1
                        }, 8, ["disabled"]),
                        Z(V.$slots, "rightbuttons", {
                          instance: (Re = Be()) == null ? void 0 : Re.exposed,
                          model: ee.value
                        })
                      ])
                    ], 2)
                  ], 34)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), u("div", b0, [
          s("p", w0, [
            xe("Could not create form for unknown "),
            x0,
            xe(" " + A(o(D)), 1)
          ])
        ])),
        ((Me = y.value) == null ? void 0 : Me.name) == "ModalLookup" && y.value.ref ? (a(), se(Ce, {
          key: 3,
          "ref-info": y.value.ref,
          onDone: v
        }, null, 8, ["ref-info"])) : k("", !0)
      ]);
    };
  }
}), z0 = { key: 0 }, N0 = { class: "text-red-700" }, U0 = /* @__PURE__ */ s("b", null, "type", -1), q0 = ["onSubmit"], Q0 = { key: 0 }, K0 = { key: 2 }, Z0 = ["innerHTML"], G0 = { class: "flex justify-end" }, W0 = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, J0 = /* @__PURE__ */ s("div", { class: "fixed inset-0" }, null, -1), X0 = { class: "fixed inset-0 overflow-hidden" }, Y0 = ["onSubmit"], ef = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, tf = { class: "flex-1" }, lf = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, nf = { class: "flex items-start justify-between space-x-3" }, sf = { class: "space-y-1" }, of = { key: 0 }, af = { key: 2 }, rf = ["innerHTML"], uf = { class: "flex h-7 items-center" }, df = { class: "flex justify-end" }, cf = /* @__PURE__ */ ue({
  __name: "AutoCreateForm",
  props: {
    type: null,
    formStyle: { default: "slideOver" },
    panelClass: null,
    formClass: null,
    headingClass: null,
    subHeadingClass: null,
    buttonsClass: null,
    heading: null,
    subHeading: null,
    autosave: { type: Boolean, default: !0 },
    showLoading: { type: Boolean, default: !0 },
    showCancel: { type: Boolean, default: !0 },
    configureField: null,
    configureFormLayout: null
  },
  emits: ["done", "save", "error"],
  setup(e, { expose: t, emit: l }) {
    const n = e, i = M(), r = M(1);
    t({ forceUpdate: d, props: n, setModel: c, formFields: i });
    function d() {
      var V, re;
      r.value++, (V = i.value) == null || V.forceUpdate();
      const F = Be();
      (re = F == null ? void 0 : F.proxy) == null || re.$forceUpdate();
    }
    function c(F) {
      Object.assign(j.value, F), d();
    }
    function m(F) {
    }
    Xt("ModalProvider", {
      openModal: p
    });
    const y = M(), b = M();
    function p(F, V) {
      y.value = F, b.value = V;
    }
    async function v(F) {
      b.value && b.value(F), y.value = void 0, b.value = void 0;
    }
    const { typeOf: g, typeProperties: O, Crud: U, createDto: Y, formValues: R } = ot(), N = f(() => Bt(n.type)), T = f(() => g(N.value)), j = M((() => typeof n.type == "string" ? Y(n.type) : n.type ? new n.type() : null)()), L = f(() => n.panelClass || Ze.panelClass(n.formStyle)), q = f(() => n.formClass || Ze.formClass(n.formStyle)), D = f(() => n.headingClass || Ze.headingClass(n.formStyle)), K = f(() => n.subHeadingClass || Ze.subHeadingClass(n.formStyle)), ne = f(() => n.buttonsClass || Ze.buttonsClass), ee = f(() => U.model(T.value)), te = f(() => {
      var F;
      return n.heading || ((F = g(N.value)) == null ? void 0 : F.description) || (ee.value ? `New ${Ee(ee.value)}` : Ee(N.value));
    }), S = M(new We());
    let le = Tl(), x = f(() => le.loading.value);
    async function W(F) {
      var fe, H;
      let V = F.target;
      if (!n.autosave) {
        l("save", new j.value.constructor(R(V, O(T.value))));
        return;
      }
      let re = Ue((fe = j.value) == null ? void 0 : fe.getMethod, (P) => typeof P == "function" ? P() : null) || "POST", ye = Ue((H = j.value) == null ? void 0 : H.createResponse, (P) => typeof P == "function" ? P() : null) == null;
      if (tn.hasRequestBody(re)) {
        let P = new j.value.constructor(), me = new FormData(V);
        ye ? S.value = await le.apiFormVoid(P, me, { jsconfig: "eccn" }) : S.value = await le.apiForm(P, me, { jsconfig: "eccn" });
      } else {
        let P = R(V, O(T.value)), me = new j.value.constructor(P);
        ye ? S.value = await le.apiVoid(me, { jsconfig: "eccn" }) : S.value = await le.api(me, { jsconfig: "eccn" });
      }
      S.value.succeeded ? (V.reset(), l("save", S.value.response)) : l("error", S.value.error);
    }
    function E() {
      l("done");
    }
    const G = M(!1), C = M(""), _ = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    Lt(G, () => {
      Ct(_, C, G.value), G.value || setTimeout(E, 700);
    }), G.value = !0;
    function B() {
      n.formStyle == "slideOver" ? G.value = !1 : E();
    }
    const de = (F) => {
      F.key === "Escape" && B();
    };
    return Xe(() => window.addEventListener("keydown", de)), Pt(() => window.removeEventListener("keydown", de)), (F, V) => {
      var Ce, ve, Ve, Fe, z, Q, oe, ge, Se;
      const re = X("AutoFormFields"), ye = X("FormLoading"), fe = X("SecondaryButton"), H = X("PrimaryButton"), P = X("CloseButton"), me = X("ModalLookup");
      return a(), u("div", null, [
        o(T) ? e.formStyle == "card" ? (a(), u("div", {
          key: 1,
          class: w(o(L))
        }, [
          s("form", {
            onSubmit: Ne(W, ["prevent"])
          }, [
            s("div", {
              class: w(o(q))
            }, [
              s("div", null, [
                F.$slots.heading ? (a(), u("div", Q0, [
                  Z(F.$slots, "heading")
                ])) : (a(), u("h3", {
                  key: 1,
                  class: w(o(D))
                }, A(o(te)), 3)),
                F.$slots.subheading ? (a(), u("div", K0, [
                  Z(F.$slots, "subheading")
                ])) : e.subHeading ? (a(), u("p", {
                  key: 3,
                  class: w(o(K))
                }, A(e.subHeading), 3)) : (Ce = o(T)) != null && Ce.notes ? (a(), u("p", {
                  key: 4,
                  class: w(["notes", o(K)]),
                  innerHTML: (ve = o(T)) == null ? void 0 : ve.notes
                }, null, 10, Z0)) : k("", !0)
              ]),
              Z(F.$slots, "header", {
                formInstance: (Ve = Be()) == null ? void 0 : Ve.exposed,
                model: j.value
              }),
              (a(), se(re, {
                ref_key: "formFields",
                ref: i,
                key: r.value,
                modelValue: j.value,
                "onUpdate:modelValue": m,
                api: S.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
              Z(F.$slots, "footer", {
                formInstance: (Fe = Be()) == null ? void 0 : Fe.exposed,
                model: j.value
              })
            ], 2),
            s("div", {
              class: w(o(ne))
            }, [
              s("div", null, [
                e.showLoading && o(x) ? (a(), se(ye, { key: 0 })) : k("", !0)
              ]),
              s("div", G0, [
                e.showCancel ? (a(), se(fe, {
                  key: 0,
                  onClick: B,
                  disabled: o(x)
                }, {
                  default: ke(() => [
                    xe("Cancel")
                  ]),
                  _: 1
                }, 8, ["disabled"])) : k("", !0),
                $e(H, {
                  type: "submit",
                  class: "ml-4",
                  disabled: o(x)
                }, {
                  default: ke(() => [
                    xe("Save")
                  ]),
                  _: 1
                }, 8, ["disabled"])
              ])
            ], 2)
          ], 40, q0)
        ], 2)) : (a(), u("div", W0, [
          J0,
          s("div", X0, [
            s("div", {
              onMousedown: B,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: V[0] || (V[0] = Ne(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: w(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", C.value])
                }, [
                  s("form", {
                    class: w(o(q)),
                    onSubmit: Ne(W, ["prevent"])
                  }, [
                    s("div", ef, [
                      s("div", tf, [
                        s("div", lf, [
                          s("div", nf, [
                            s("div", sf, [
                              F.$slots.heading ? (a(), u("div", of, [
                                Z(F.$slots, "heading")
                              ])) : (a(), u("h3", {
                                key: 1,
                                class: w(o(D))
                              }, A(o(te)), 3)),
                              F.$slots.subheading ? (a(), u("div", af, [
                                Z(F.$slots, "subheading")
                              ])) : e.subHeading ? (a(), u("p", {
                                key: 3,
                                class: w(o(K))
                              }, A(e.subHeading), 3)) : (z = o(T)) != null && z.notes ? (a(), u("p", {
                                key: 4,
                                class: w(["notes", o(K)]),
                                innerHTML: (Q = o(T)) == null ? void 0 : Q.notes
                              }, null, 10, rf)) : k("", !0)
                            ]),
                            s("div", uf, [
                              $e(P, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: B
                              })
                            ])
                          ])
                        ]),
                        Z(F.$slots, "header", {
                          formInstance: (oe = Be()) == null ? void 0 : oe.exposed,
                          model: j.value
                        }),
                        (a(), se(re, {
                          ref_key: "formFields",
                          ref: i,
                          key: r.value,
                          modelValue: j.value,
                          "onUpdate:modelValue": m,
                          api: S.value,
                          configureField: e.configureField,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
                        Z(F.$slots, "footer", {
                          formInstance: (ge = Be()) == null ? void 0 : ge.exposed,
                          model: j.value
                        })
                      ])
                    ]),
                    s("div", {
                      class: w(o(ne))
                    }, [
                      s("div", null, [
                        e.showLoading && o(x) ? (a(), se(ye, { key: 0 })) : k("", !0)
                      ]),
                      s("div", df, [
                        e.showCancel ? (a(), se(fe, {
                          key: 0,
                          onClick: B,
                          disabled: o(x)
                        }, {
                          default: ke(() => [
                            xe("Cancel")
                          ]),
                          _: 1
                        }, 8, ["disabled"])) : k("", !0),
                        $e(H, {
                          type: "submit",
                          class: "ml-4",
                          disabled: o(x)
                        }, {
                          default: ke(() => [
                            xe("Save")
                          ]),
                          _: 1
                        }, 8, ["disabled"])
                      ])
                    ], 2)
                  ], 42, Y0)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), u("div", z0, [
          s("p", N0, [
            xe("Could not create form for unknown "),
            U0,
            xe(" " + A(o(N)), 1)
          ])
        ])),
        ((Se = y.value) == null ? void 0 : Se.name) == "ModalLookup" && y.value.ref ? (a(), se(me, {
          key: 3,
          "ref-info": y.value.ref,
          onDone: v
        }, null, 8, ["ref-info"])) : k("", !0)
      ]);
    };
  }
}), ff = { key: 0 }, mf = { class: "text-red-700" }, vf = /* @__PURE__ */ s("b", null, "type", -1), hf = ["onSubmit"], gf = { key: 0 }, pf = { key: 2 }, yf = ["innerHTML"], bf = { class: "flex justify-end" }, wf = {
  key: 2,
  class: "relative z-10",
  "aria-labelledby": "slide-over-title",
  role: "dialog",
  "aria-modal": "true"
}, xf = /* @__PURE__ */ s("div", { class: "fixed inset-0" }, null, -1), kf = { class: "fixed inset-0 overflow-hidden" }, $f = ["onSubmit"], Cf = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, _f = { class: "flex-1" }, Lf = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, Vf = { class: "flex items-start justify-between space-x-3" }, Sf = { class: "space-y-1" }, Mf = { key: 0 }, Af = { key: 2 }, Tf = ["innerHTML"], Ff = { class: "flex h-7 items-center" }, If = { class: "flex justify-end" }, Df = /* @__PURE__ */ ue({
  __name: "AutoEditForm",
  props: {
    modelValue: null,
    type: null,
    deleteType: null,
    formStyle: { default: "slideOver" },
    panelClass: null,
    formClass: null,
    headingClass: null,
    subHeadingClass: null,
    heading: null,
    subHeading: null,
    autosave: { type: Boolean, default: !0 },
    showLoading: { type: Boolean, default: !0 },
    configureField: null,
    configureFormLayout: null
  },
  emits: ["done", "save", "delete", "error"],
  setup(e, { expose: t, emit: l }) {
    const n = e, i = M(), r = M(1);
    t({ forceUpdate: d, props: n, setModel: c, formFields: i });
    function d() {
      var P;
      r.value++, D.value = q();
      const H = Be();
      (P = H == null ? void 0 : H.proxy) == null || P.$forceUpdate();
    }
    function c(H) {
      Object.assign(D.value, H);
    }
    function m(H) {
    }
    Xt("ModalProvider", {
      openModal: p
    });
    const y = M(), b = M();
    function p(H, P) {
      y.value = H, b.value = P;
    }
    async function v(H) {
      b.value && b.value(H), y.value = void 0, b.value = void 0;
    }
    const { typeOf: g, apiOf: O, typeProperties: U, createFormLayout: Y, getPrimaryKey: R, Crud: N, createDto: T, formValues: J } = ot(), j = f(() => Bt(n.type)), L = f(() => g(j.value)), q = () => typeof n.type == "string" ? T(n.type, ml(n.modelValue)) : n.type ? new n.type(ml(n.modelValue)) : null, D = M(q()), K = f(() => n.panelClass || Ze.panelClass(n.formStyle)), ne = f(() => n.formClass || Ze.formClass(n.formStyle)), ee = f(() => n.headingClass || Ze.headingClass(n.formStyle)), te = f(() => n.subHeadingClass || Ze.subHeadingClass(n.formStyle)), S = f(() => N.model(L.value)), le = f(() => {
      var H;
      return n.heading || ((H = g(j.value)) == null ? void 0 : H.description) || (S.value ? `Update ${Ee(S.value)}` : Ee(j.value));
    }), x = M(new We());
    let W = Tl(), E = f(() => W.loading.value);
    const G = () => Ue(g(N.model(L.value)), (H) => R(H));
    function C(H) {
      const { op: P, prop: me } = H;
      P && (N.isPatch(P) || N.isUpdate(P)) && (H.disabled = me == null ? void 0 : me.isPrimaryKey), n.configureField && n.configureField(H);
    }
    async function _(H) {
      var Ve, Fe;
      let P = H.target;
      if (!n.autosave) {
        l("save", new D.value.constructor(J(P, U(L.value))));
        return;
      }
      let me = Ue((Ve = D.value) == null ? void 0 : Ve.getMethod, (z) => typeof z == "function" ? z() : null) || "POST", Ce = Ue((Fe = D.value) == null ? void 0 : Fe.createResponse, (z) => typeof z == "function" ? z() : null) == null, ve = G();
      if (tn.hasRequestBody(me)) {
        let z = new D.value.constructor(), Q = be(n.modelValue, ve.name), oe = new FormData(P);
        ve && !Array.from(oe.keys()).some((Re) => Re.toLowerCase() == ve.name.toLowerCase()) && oe.append(ve.name, Q);
        let ge = [];
        const Se = j.value && O(j.value);
        if (Se && N.isPatch(Se)) {
          let Re = ml(n.modelValue), Me = Y(L.value), De = {};
          if (ve && (De[ve.name] = Q), Me.forEach((je) => {
            let Ye = je.id, Ht = be(Re, Ye);
            if (ve && ve.name.toLowerCase() === Ye.toLowerCase())
              return;
            let mt = oe.get(Ye), ll = mt != null, nl = je.type === "checkbox" ? ll !== !!Ht : je.type === "file" ? ll : mt != Ht;
            !mt && !Ht && (nl = !1), nl && (mt ? De[Ye] = mt : je.type !== "file" && ge.push(Ye));
          }), Array.from(oe.keys()).filter((je) => !De[je]).forEach((je) => oe.delete(je)), Array.from(oe.keys()).filter((je) => je.toLowerCase() != ve.name.toLowerCase()).length == 0 && ge.length == 0) {
            ye();
            return;
          }
        }
        const Oe = ge.length > 0 ? { jsconfig: "eccn", reset: ge } : { jsconfig: "eccn" };
        Ce ? x.value = await W.apiFormVoid(z, oe, Oe) : x.value = await W.apiForm(z, oe, Oe);
      } else {
        let z = J(P, U(L.value));
        ve && !be(z, ve.name) && (z[ve.name] = be(n.modelValue, ve.name));
        let Q = new D.value.constructor(z);
        Ce ? x.value = await W.apiVoid(Q, { jsconfig: "eccn" }) : x.value = await W.api(Q, { jsconfig: "eccn" });
      }
      x.value.succeeded ? (P.reset(), l("save", x.value.response)) : l("error", x.value.error);
    }
    async function B(H) {
      let P = G();
      const me = P ? be(n.modelValue, P.name) : null;
      if (!me) {
        console.error(`Could not find Primary Key for Type ${j.value} (${S.value})`);
        return;
      }
      const Ce = { [P.name]: me }, ve = typeof n.deleteType == "string" ? T(n.deleteType, Ce) : n.deleteType ? new n.deleteType(Ce) : null;
      Ue(ve.createResponse, (Fe) => typeof Fe == "function" ? Fe() : null) == null ? x.value = await W.apiVoid(ve) : x.value = await W.api(ve), x.value.succeeded ? l("delete", x.value.response) : l("error", x.value.error);
    }
    function de() {
      l("done");
    }
    const F = M(!1), V = M(""), re = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    Lt(F, () => {
      Ct(re, V, F.value), F.value || setTimeout(de, 700);
    }), F.value = !0;
    function ye() {
      n.formStyle == "slideOver" ? F.value = !1 : de();
    }
    const fe = (H) => {
      H.key === "Escape" && ye();
    };
    return Xe(() => window.addEventListener("keydown", fe)), Pt(() => window.removeEventListener("keydown", fe)), (H, P) => {
      var oe, ge, Se, Oe, Re, Me, De, Qe, je;
      const me = X("AutoFormFields"), Ce = X("ConfirmDelete"), ve = X("FormLoading"), Ve = X("SecondaryButton"), Fe = X("PrimaryButton"), z = X("CloseButton"), Q = X("ModalLookup");
      return a(), u("div", null, [
        o(L) ? e.formStyle == "card" ? (a(), u("div", {
          key: 1,
          class: w(o(K))
        }, [
          s("form", {
            onSubmit: Ne(_, ["prevent"])
          }, [
            s("div", {
              class: w(o(ne))
            }, [
              s("div", null, [
                H.$slots.heading ? (a(), u("div", gf, [
                  Z(H.$slots, "heading")
                ])) : (a(), u("h3", {
                  key: 1,
                  class: w(o(ee))
                }, A(o(le)), 3)),
                H.$slots.subheading ? (a(), u("div", pf, [
                  Z(H.$slots, "subheading")
                ])) : e.subHeading ? (a(), u("p", {
                  key: 3,
                  class: w(o(te))
                }, A(e.subHeading), 3)) : (oe = o(L)) != null && oe.notes ? (a(), u("p", {
                  key: 4,
                  class: w(["notes", o(te)]),
                  innerHTML: (ge = o(L)) == null ? void 0 : ge.notes
                }, null, 10, yf)) : k("", !0)
              ]),
              Z(H.$slots, "header", {
                formInstance: (Se = Be()) == null ? void 0 : Se.exposed,
                model: D.value
              }),
              (a(), se(me, {
                ref_key: "formFields",
                ref: i,
                key: r.value,
                modelValue: D.value,
                "onUpdate:modelValue": m,
                api: x.value,
                configureField: e.configureField,
                configureFormLayout: e.configureFormLayout
              }, null, 8, ["modelValue", "api", "configureField", "configureFormLayout"])),
              Z(H.$slots, "footer", {
                formInstance: (Oe = Be()) == null ? void 0 : Oe.exposed,
                model: D.value
              })
            ], 2),
            s("div", {
              class: w(o(Ze).buttonsClass)
            }, [
              s("div", null, [
                e.deleteType ? (a(), se(Ce, {
                  key: 0,
                  onDelete: B
                })) : k("", !0)
              ]),
              s("div", null, [
                e.showLoading && o(E) ? (a(), se(ve, { key: 0 })) : k("", !0)
              ]),
              s("div", bf, [
                $e(Ve, {
                  onClick: ye,
                  disabled: o(E)
                }, {
                  default: ke(() => [
                    xe("Cancel")
                  ]),
                  _: 1
                }, 8, ["disabled"]),
                $e(Fe, {
                  type: "submit",
                  class: "ml-4",
                  disabled: o(E)
                }, {
                  default: ke(() => [
                    xe("Save")
                  ]),
                  _: 1
                }, 8, ["disabled"])
              ])
            ], 2)
          ], 40, hf)
        ], 2)) : (a(), u("div", wf, [
          xf,
          s("div", kf, [
            s("div", {
              onMousedown: ye,
              class: "absolute inset-0 overflow-hidden"
            }, [
              s("div", {
                onMousedown: P[0] || (P[0] = Ne(() => {
                }, ["stop"])),
                class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
              }, [
                s("div", {
                  class: w(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", V.value])
                }, [
                  s("form", {
                    class: w(o(ne)),
                    onSubmit: Ne(_, ["prevent"])
                  }, [
                    s("div", Cf, [
                      s("div", _f, [
                        s("div", Lf, [
                          s("div", Vf, [
                            s("div", Sf, [
                              H.$slots.heading ? (a(), u("div", Mf, [
                                Z(H.$slots, "heading")
                              ])) : (a(), u("h3", {
                                key: 1,
                                class: w(o(ee))
                              }, A(o(le)), 3)),
                              H.$slots.subheading ? (a(), u("div", Af, [
                                Z(H.$slots, "subheading")
                              ])) : e.subHeading ? (a(), u("p", {
                                key: 3,
                                class: w(o(te))
                              }, A(e.subHeading), 3)) : (Re = o(L)) != null && Re.notes ? (a(), u("p", {
                                key: 4,
                                class: w(["notes", o(te)]),
                                innerHTML: (Me = o(L)) == null ? void 0 : Me.notes
                              }, null, 10, Tf)) : k("", !0)
                            ]),
                            s("div", Ff, [
                              $e(z, {
                                "button-class": "bg-gray-50 dark:bg-gray-900",
                                onClose: ye
                              })
                            ])
                          ])
                        ]),
                        Z(H.$slots, "header", {
                          formInstance: (De = Be()) == null ? void 0 : De.exposed,
                          model: D.value
                        }),
                        (a(), se(me, {
                          ref_key: "formFields",
                          ref: i,
                          key: r.value,
                          modelValue: D.value,
                          "onUpdate:modelValue": m,
                          api: x.value,
                          configureField: C,
                          configureFormLayout: e.configureFormLayout
                        }, null, 8, ["modelValue", "api", "configureFormLayout"])),
                        Z(H.$slots, "footer", {
                          formInstance: (Qe = Be()) == null ? void 0 : Qe.exposed,
                          model: D.value
                        })
                      ])
                    ]),
                    s("div", {
                      class: w(o(Ze).buttonsClass)
                    }, [
                      s("div", null, [
                        e.deleteType ? (a(), se(Ce, {
                          key: 0,
                          onDelete: B
                        })) : k("", !0)
                      ]),
                      s("div", null, [
                        e.showLoading && o(E) ? (a(), se(ve, { key: 0 })) : k("", !0)
                      ]),
                      s("div", If, [
                        $e(Ve, {
                          onClick: ye,
                          disabled: o(E)
                        }, {
                          default: ke(() => [
                            xe("Cancel")
                          ]),
                          _: 1
                        }, 8, ["disabled"]),
                        $e(Fe, {
                          type: "submit",
                          class: "ml-4",
                          disabled: o(E)
                        }, {
                          default: ke(() => [
                            xe("Save")
                          ]),
                          _: 1
                        }, 8, ["disabled"])
                      ])
                    ], 2)
                  ], 42, $f)
                ], 2)
              ], 32)
            ], 32)
          ])
        ])) : (a(), u("div", ff, [
          s("p", mf, [
            xe("Could not create form for unknown "),
            vf,
            xe(" " + A(o(j)), 1)
          ])
        ])),
        ((je = y.value) == null ? void 0 : je.name) == "ModalLookup" && y.value.ref ? (a(), se(Q, {
          key: 3,
          "ref-info": y.value.ref,
          onDone: v
        }, null, 8, ["ref-info"])) : k("", !0)
      ]);
    };
  }
}), Of = /* @__PURE__ */ s("label", {
  for: "confirmDelete",
  class: "ml-2 mr-2 select-none"
}, "confirm", -1), jf = ["onClick"], Pf = /* @__PURE__ */ ue({
  __name: "ConfirmDelete",
  emits: ["delete"],
  setup(e, { emit: t }) {
    let l = M(!1);
    const n = () => {
      l.value && t("delete");
    }, i = f(() => [
      "select-none inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white",
      l.value ? "cursor-pointer bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500" : "bg-red-400"
    ]);
    return (r, d) => (a(), u(Te, null, [
      kt(s("input", {
        id: "confirmDelete",
        type: "checkbox",
        class: "focus:ring-indigo-500 h-4 w-4 text-indigo-600 rounded border-gray-300 dark:border-gray-600 dark:bg-gray-800 dark:ring-offset-black",
        "onUpdate:modelValue": d[0] || (d[0] = (c) => qn(l) ? l.value = c : l = c)
      }, null, 512), [
        [Xl, o(l)]
      ]),
      Of,
      s("span", Le({
        onClick: Ne(n, ["prevent"]),
        class: o(i)
      }, r.$attrs), [
        Z(r.$slots, "default", {}, () => [
          xe("Delete")
        ])
      ], 16, jf)
    ], 64));
  }
}), Bf = {
  class: "flex",
  title: "loading..."
}, Rf = {
  key: 0,
  xmlns: "http://www.w3.org/2000/svg",
  x: "0px",
  y: "0px",
  width: "24px",
  height: "30px",
  viewBox: "0 0 24 30"
}, Hf = /* @__PURE__ */ wl('<rect x="0" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0s" dur="0.6s" repeatCount="indefinite"></animate></rect><rect x="8" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0.15s" dur="0.6s" repeatCount="indefinite"></animate></rect><rect x="16" y="10" width="4" height="10" fill="#333" opacity="0.2"><animate attributeName="opacity" attributeType="XML" values="0.2; 1; .2" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="height" attributeType="XML" values="10; 20; 10" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate><animate attributeName="y" attributeType="XML" values="10; 5; 10" begin="0.3s" dur="0.6s" repeatCount="indefinite"></animate></rect>', 3), Ef = [
  Hf
], zf = { class: "ml-2 mt-1 text-gray-400" }, Nf = /* @__PURE__ */ ue({
  __name: "FormLoading",
  props: {
    icon: { type: Boolean, default: !0 },
    text: { default: "loading..." }
  },
  setup(e) {
    return qe("ApiState", void 0), (t, l) => (a(), u("div", Bf, [
      e.icon ? (a(), u("svg", Rf, Ef)) : k("", !0),
      s("span", zf, A(e.text), 1)
    ]));
  }
}), Uf = ["onClick"], qf = {
  key: 3,
  class: "flex justify-between items-center"
}, Qf = { class: "mr-1 select-none" }, Kf = ["onClick"], Zf = /* @__PURE__ */ ue({
  __name: "DataGrid",
  props: {
    items: { default: () => [] },
    id: { default: "DataGrid" },
    type: null,
    tableStyle: { default: "stripedRows" },
    selectedColumns: null,
    gridClass: null,
    grid2Class: null,
    grid3Class: null,
    grid4Class: null,
    tableClass: null,
    theadClass: null,
    tbodyClass: null,
    theadRowClass: null,
    theadCellClass: null,
    isSelected: null,
    headerTitle: null,
    headerTitles: null,
    visibleFrom: null,
    rowClass: null,
    rowStyle: null
  },
  emits: ["headerSelected", "rowSelected"],
  setup(e, { emit: t }) {
    const l = e, n = M(), i = M(null), r = (E) => i.value === E, d = Yl(), c = (E) => Object.keys(d).find((G) => G.toLowerCase() == E.toLowerCase() + "-header"), m = (E) => Object.keys(d).find((G) => G.toLowerCase() == E.toLowerCase()), h = f(() => zl(l.items).filter((E) => !!(d[E] || d[E + "-header"]))), { typeOf: y, typeProperties: b } = ot(), p = f(() => Bt(l.type)), v = f(() => y(p.value)), g = f(() => b(v.value));
    function O(E) {
      const G = l.headerTitles && be(l.headerTitles, E) || E;
      return l.headerTitle ? l.headerTitle(G) : Jn(G);
    }
    function U(E) {
      const G = E.toLowerCase();
      return g.value.find((C) => C.name.toLowerCase() == G);
    }
    function Y(E) {
      const G = U(E);
      return G != null && G.format ? G.format : (G == null ? void 0 : G.type) == "TimeSpan" || (G == null ? void 0 : G.type) == "TimeOnly" ? { method: "time" } : null;
    }
    const R = {
      xs: "xs:table-cell",
      sm: "sm:table-cell",
      md: "md:table-cell",
      lg: "lg:table-cell",
      xl: "xl:table-cell",
      "2xl": "2xl:table-cell",
      never: ""
    };
    function N(E) {
      const G = l.visibleFrom && be(l.visibleFrom, E);
      return G && Ue(R[G], (C) => `hidden ${C}`);
    }
    const T = f(() => l.gridClass ?? he.getGridClass(l.tableStyle)), J = f(() => l.grid2Class ?? he.getGrid2Class(l.tableStyle)), j = f(() => l.grid3Class ?? he.getGrid3Class(l.tableStyle)), L = f(() => l.grid4Class ?? he.getGrid4Class(l.tableStyle)), q = f(() => l.tableClass ?? he.getTableClass(l.tableStyle)), D = f(() => l.tbodyClass ?? he.getTbodyClass(l.tbodyClass)), K = f(() => l.theadClass ?? he.getTheadClass(l.tableStyle)), ne = f(() => l.theadRowClass ?? he.getTheadRowClass(l.tableStyle)), ee = f(() => l.theadCellClass ?? he.getTheadCellClass(l.tableStyle));
    function te(E, G) {
      return l.rowClass ? l.rowClass(E, G) : he.getTableRowClass(l.tableStyle, G, !!(l.isSelected && l.isSelected(E)), l.isSelected != null);
    }
    function S(E, G) {
      return l.rowStyle ? l.rowStyle(E, G) : void 0;
    }
    const le = f(() => {
      const E = (typeof l.selectedColumns == "string" ? l.selectedColumns.split(",") : l.selectedColumns) || (h.value.length > 0 ? h.value : zl(l.items)), G = g.value.reduce((C, _) => (C[_.name.toLowerCase()] = _.format, C), {});
      return E.filter((C) => {
        var _;
        return ((_ = G[C.toLowerCase()]) == null ? void 0 : _.method) != "hidden";
      });
    });
    function x(E, G) {
      t("headerSelected", G, E);
    }
    function W(E, G, C) {
      t("rowSelected", C, E);
    }
    return (E, G) => {
      const C = X("CellFormat"), _ = X("PreviewFormat");
      return e.items.length ? (a(), u("div", {
        key: 0,
        ref_key: "refResults",
        ref: n,
        class: w(o(T))
      }, [
        s("div", {
          class: w(o(J))
        }, [
          s("div", {
            class: w(o(j))
          }, [
            s("div", {
              class: w(o(L))
            }, [
              s("table", {
                class: w(o(q))
              }, [
                s("thead", {
                  class: w(o(K))
                }, [
                  s("tr", {
                    class: w(o(ne))
                  }, [
                    (a(!0), u(Te, null, Ie(o(le), (B) => (a(), u("td", {
                      class: w([N(B), o(ee), r(B) ? "text-gray-900 dark:text-gray-50" : "text-gray-500 dark:text-gray-400"])
                    }, [
                      s("div", {
                        onClick: (de) => x(de, B)
                      }, [
                        o(d)[B + "-header"] ? Z(E.$slots, B + "-header", {
                          key: 0,
                          column: B
                        }) : c(B) ? Z(E.$slots, c(B), {
                          key: 1,
                          column: B
                        }) : o(d).header ? Z(E.$slots, "header", {
                          key: 2,
                          column: B,
                          label: O(B)
                        }) : (a(), u("div", qf, [
                          s("span", Qf, A(O(B)), 1)
                        ]))
                      ], 8, Uf)
                    ], 2))), 256))
                  ], 2)
                ], 2),
                s("tbody", {
                  class: w(o(D))
                }, [
                  (a(!0), u(Te, null, Ie(e.items, (B, de) => (a(), u("tr", {
                    class: w(te(B, de)),
                    style: Jl(S(B, de)),
                    onClick: (F) => W(F, de, B)
                  }, [
                    (a(!0), u(Te, null, Ie(o(le), (F) => (a(), u("td", {
                      class: w([N(F), o(he).tableCellClass])
                    }, [
                      o(d)[F] ? Z(E.$slots, F, It(Le({ key: 0 }, B))) : m(F) ? Z(E.$slots, m(F), It(Le({ key: 1 }, B))) : U(F) ? (a(), se(C, {
                        key: 2,
                        type: o(v),
                        propType: U(F),
                        modelValue: B
                      }, null, 8, ["type", "propType", "modelValue"])) : (a(), se(_, {
                        key: 3,
                        value: o(be)(B, F),
                        format: Y(F)
                      }, null, 8, ["value", "format"]))
                    ], 2))), 256))
                  ], 14, Kf))), 256))
                ], 2)
              ], 2)
            ], 2)
          ], 2)
        ], 2)
      ], 2)) : k("", !0);
    };
  }
}), Gf = ue({
  props: {
    type: Object,
    propType: Object,
    modelValue: Object
  },
  setup(e, { attrs: t }) {
    const { typeOf: l } = ot();
    function n(i) {
      return i != null && i.format ? i.format : (i == null ? void 0 : i.type) == "TimeSpan" || (i == null ? void 0 : i.type) == "TimeOnly" ? { method: "time" } : null;
    }
    return () => {
      var Y;
      const i = n(e.propType), r = be(e.modelValue, e.propType.name), d = Object.assign({}, e, t), c = ht("span", { innerHTML: Jt(r, i, d) }), m = Zt(r) && Array.isArray(r) ? ht("span", {}, [
        ht("span", { class: "mr-2" }, `${r.length}`),
        c
      ]) : c, h = (Y = e.propType) == null ? void 0 : Y.ref;
      if (!h)
        return m;
      const b = Je(e.type).find((R) => R.type === h.model);
      if (!b)
        return m;
      const p = be(e.modelValue, b.name), v = p && h.refLabel && be(p, h.refLabel);
      if (!v)
        return m;
      const g = l(h.model), O = g == null ? void 0 : g.icon, U = O ? ht(Rs, { image: O, class: "w-5 h-5 mr-1" }) : null;
      return ht("span", { class: "flex", title: `${h.model} ${r}` }, [
        U,
        v
      ]);
    };
  }
}), Wf = { key: 0 }, Jf = {
  key: 0,
  class: "mr-2"
}, Xf = ["innerHTML"], Yf = ["innerHTML"], e1 = {
  inheritAttrs: !1
}, t1 = /* @__PURE__ */ ue({
  ...e1,
  __name: "PreviewFormat",
  props: {
    value: null,
    format: null,
    includeIcon: { type: Boolean, default: !0 },
    includeCount: { type: Boolean, default: !0 },
    maxFieldLength: { default: 150 },
    maxNestedFields: { default: 2 },
    maxNestedFieldLength: { default: 30 }
  },
  setup(e) {
    const t = e, l = f(() => Array.isArray(t.value));
    return (n, i) => o(Zt)(e.value) ? (a(), u("span", Wf, [
      e.includeCount && o(l) ? (a(), u("span", Jf, A(e.value.length), 1)) : k("", !0),
      s("span", {
        innerHTML: o(Jt)(e.value, e.format, n.$attrs)
      }, null, 8, Xf)
    ])) : (a(), u("span", {
      key: 1,
      innerHTML: o(Jt)(e.value, e.format, n.$attrs)
    }, null, 8, Yf));
  }
}), l1 = ["innerHTML"], n1 = { key: 0 }, s1 = /* @__PURE__ */ s("b", null, null, -1), o1 = { key: 2 }, a1 = /* @__PURE__ */ ue({
  __name: "HtmlFormat",
  props: {
    value: null,
    depth: { default: 0 },
    fieldAttrs: null,
    classes: { type: Function, default: (e, t, l, n, i) => n }
  },
  setup(e) {
    const t = e, l = f(() => _t(t.value)), n = f(() => Array.isArray(t.value)), i = (m) => Jn(m), r = (m) => t.fieldAttrs ? t.fieldAttrs(m) : null, d = f(() => zl(t.value)), c = (m) => m ? Object.keys(m).map((h) => ({ key: i(h), val: m[h] })) : [];
    return (m, h) => {
      const y = X("HtmlFormat", !0);
      return a(), u("div", {
        class: w(e.depth == 0 ? "prose html-format" : "")
      }, [
        o(l) ? (a(), u("div", {
          key: 0,
          innerHTML: o(Jt)(e.value)
        }, null, 8, l1)) : o(n) ? (a(), u("div", {
          key: 1,
          class: w(e.classes("array", "div", e.depth, o(he).gridClass))
        }, [
          o(_t)(e.value[0]) ? (a(), u("div", n1, "[ " + A(e.value.join(", ")) + " ]", 1)) : (a(), u("div", {
            key: 1,
            class: w(e.classes("array", "div", e.depth, o(he).grid2Class))
          }, [
            s("div", {
              class: w(e.classes("array", "div", e.depth, o(he).grid3Class))
            }, [
              s("div", {
                class: w(e.classes("array", "div", e.depth, o(he).grid4Class))
              }, [
                s("table", {
                  class: w(e.classes("object", "table", e.depth, o(he).tableClass))
                }, [
                  s("thead", {
                    class: w(e.classes("array", "thead", e.depth, o(he).theadClass))
                  }, [
                    s("tr", null, [
                      (a(!0), u(Te, null, Ie(o(d), (b) => (a(), u("th", {
                        class: w(e.classes("array", "th", e.depth, o(he).theadCellClass + " whitespace-nowrap"))
                      }, [
                        s1,
                        xe(A(i(b)), 1)
                      ], 2))), 256))
                    ])
                  ], 2),
                  s("tbody", null, [
                    (a(!0), u(Te, null, Ie(e.value, (b, p) => (a(), u("tr", {
                      class: w(e.classes("array", "tr", e.depth, p % 2 == 0 ? "bg-white" : "bg-gray-50", p))
                    }, [
                      (a(!0), u(Te, null, Ie(o(d), (v) => (a(), u("td", {
                        class: w(e.classes("array", "td", e.depth, o(he).tableCellClass))
                      }, [
                        $e(y, Le({
                          value: b[v],
                          "field-attrs": e.fieldAttrs,
                          depth: e.depth + 1,
                          classes: e.classes
                        }, r(v)), null, 16, ["value", "field-attrs", "depth", "classes"])
                      ], 2))), 256))
                    ], 2))), 256))
                  ])
                ], 2)
              ], 2)
            ], 2)
          ], 2))
        ], 2)) : (a(), u("div", o1, [
          s("table", {
            class: w(e.classes("object", "table", e.depth, "table-object"))
          }, [
            (a(!0), u(Te, null, Ie(c(e.value), (b) => (a(), u("tr", {
              class: w(e.classes("object", "tr", e.depth, ""))
            }, [
              s("th", {
                class: w(e.classes("object", "th", e.depth, "align-top py-2 px-4 text-left text-sm font-medium tracking-wider whitespace-nowrap"))
              }, A(b.key), 3),
              s("td", {
                class: w(e.classes("object", "td", e.depth, "align-top py-2 px-4 text-sm"))
              }, [
                $e(y, Le({
                  value: b.val,
                  "field-attrs": e.fieldAttrs,
                  depth: e.depth + 1,
                  classes: e.classes
                }, r(b.key)), null, 16, ["value", "field-attrs", "depth", "classes"])
              ], 2)
            ], 2))), 256))
          ], 2)
        ]))
      ], 2);
    };
  }
}), i1 = { class: "absolute top-0 right-0 pt-4 pr-4" }, r1 = /* @__PURE__ */ s("span", { class: "sr-only" }, "Close", -1), u1 = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6",
  xmlns: "http://www.w3.org/2000/svg",
  fill: "none",
  viewBox: "0 0 24 24",
  stroke: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-linejoin": "round",
    "stroke-width": "2",
    d: "M6 18L18 6M6 6l12 12"
  })
], -1), d1 = [
  r1,
  u1
], c1 = /* @__PURE__ */ ue({
  __name: "CloseButton",
  props: {
    buttonClass: { default: "bg-white dark:bg-black" }
  },
  emits: ["close"],
  setup(e, { emit: t }) {
    return (l, n) => (a(), u("div", i1, [
      s("button", {
        type: "button",
        onClick: n[0] || (n[0] = (i) => l.$emit("close")),
        class: w([e.buttonClass, "rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black"])
      }, d1, 2)
    ]));
  }
}), f1 = ["id", "aria-labelledby"], m1 = /* @__PURE__ */ s("div", { class: "fixed inset-0" }, null, -1), v1 = { class: "fixed inset-0 overflow-hidden" }, h1 = { class: "flex h-full flex-col bg-white dark:bg-black shadow-xl" }, g1 = { class: "flex min-h-0 flex-1 flex-col overflow-auto" }, p1 = { class: "flex-1" }, y1 = { class: "bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6" }, b1 = { class: "flex items-start justify-between space-x-3" }, w1 = { class: "space-y-1" }, x1 = ["id"], k1 = {
  key: 1,
  class: "text-sm text-gray-500"
}, $1 = { class: "flex h-7 items-center" }, C1 = { class: "flex-shrink-0 border-t border-gray-200 dark:border-gray-700 px-4 py-5 sm:px-6" }, _1 = /* @__PURE__ */ ue({
  __name: "SlideOver",
  props: {
    id: { default: "SlideOver" },
    title: null,
    contentClass: { default: "relative mt-6 flex-1 px-4 sm:px-6" }
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = M(!1), n = M(""), i = {
      entering: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transform transition ease-in-out duration-500 sm:duration-700", from: "translate-x-0", to: "translate-x-full" }
    };
    Lt(l, () => {
      Ct(i, n, l.value), l.value || setTimeout(() => t("done"), 700);
    }), l.value = !0;
    const r = () => l.value = !1, d = (c) => {
      c.key === "Escape" && r();
    };
    return Xe(() => window.addEventListener("keydown", d)), Pt(() => window.removeEventListener("keydown", d)), (c, m) => {
      const h = X("CloseButton");
      return a(), u("div", {
        id: e.id,
        class: "relative z-10",
        "aria-labelledby": e.id + "-title",
        role: "dialog",
        "aria-modal": "true"
      }, [
        m1,
        s("div", v1, [
          s("div", {
            onMousedown: r,
            class: "absolute inset-0 overflow-hidden"
          }, [
            s("div", {
              onMousedown: m[0] || (m[0] = Ne(() => {
              }, ["stop"])),
              class: "pointer-events-none fixed inset-y-0 right-0 flex pl-10"
            }, [
              s("div", {
                class: w(["pointer-events-auto w-screen xl:max-w-3xl md:max-w-xl max-w-lg", n.value])
              }, [
                s("div", h1, [
                  s("div", g1, [
                    s("div", p1, [
                      s("div", y1, [
                        s("div", b1, [
                          s("div", w1, [
                            e.title ? (a(), u("h2", {
                              key: 0,
                              class: "text-lg font-medium text-gray-900 dark:text-gray-50",
                              id: e.id + "-title"
                            }, A(e.title), 9, x1)) : k("", !0),
                            c.$slots.subtitle ? (a(), u("p", k1, [
                              Z(c.$slots, "subtitle")
                            ])) : k("", !0)
                          ]),
                          s("div", $1, [
                            $e(h, {
                              "button-class": "bg-gray-50 dark:bg-gray-900",
                              onClose: r
                            })
                          ])
                        ])
                      ]),
                      s("div", {
                        class: w(e.contentClass)
                      }, [
                        Z(c.$slots, "default")
                      ], 2)
                    ])
                  ]),
                  s("div", C1, [
                    Z(c.$slots, "footer")
                  ])
                ])
              ], 2)
            ], 32)
          ], 32)
        ])
      ], 8, f1);
    };
  }
}), L1 = ["id", "data-transition-for", "aria-labelledby"], V1 = { class: "fixed inset-0 z-10 overflow-y-auto" }, S1 = { class: "flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0" }, M1 = /* @__PURE__ */ s("span", { class: "sr-only" }, "Close", -1), A1 = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6",
  xmlns: "http://www.w3.org/2000/svg",
  fill: "none",
  viewBox: "0 0 24 24",
  stroke: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-linejoin": "round",
    "stroke-width": "2",
    d: "M6 18L18 6M6 6l12 12"
  })
], -1), T1 = [
  M1,
  A1
], F1 = /* @__PURE__ */ ue({
  __name: "ModalDialog",
  props: {
    id: { default: "ModalDialog" },
    modalClass: { default: Wl.modalClass },
    sizeClass: { default: Wl.sizeClass }
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = M(!1), n = M(""), i = {
      entering: { cls: "ease-out duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "ease-in duration-200", from: "opacity-100", to: "opacity-0" }
    }, r = M(""), d = {
      entering: { cls: "ease-out duration-300", from: "opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95", to: "opacity-100 translate-y-0 sm:scale-100" },
      leaving: { cls: "ease-in duration-200", from: "opacity-100 translate-y-0 sm:scale-100", to: "opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95" }
    };
    Lt(l, () => {
      Ct(i, n, l.value), Ct(d, r, l.value), l.value || setTimeout(() => t("done"), 200);
    }), l.value = !0;
    const c = () => l.value = !1;
    Xt("ModalProvider", {
      openModal: b
    });
    const h = M(), y = M();
    function b(g, O) {
      h.value = g, y.value = O;
    }
    async function p(g) {
      y.value && y.value(g), h.value = void 0, y.value = void 0;
    }
    const v = (g) => {
      g.key === "Escape" && c();
    };
    return Xe(() => window.addEventListener("keydown", v)), Pt(() => window.removeEventListener("keydown", v)), (g, O) => {
      var Y;
      const U = X("ModalLookup");
      return a(), u("div", {
        id: e.id,
        "data-transition-for": e.id,
        onMousedown: c,
        class: "relative z-10",
        "aria-labelledby": `${e.id}-title`,
        role: "dialog",
        "aria-modal": "true"
      }, [
        s("div", {
          class: w(["fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity", n.value])
        }, null, 2),
        s("div", V1, [
          s("div", S1, [
            s("div", {
              class: w([e.modalClass, e.sizeClass, r.value]),
              onMousedown: O[0] || (O[0] = Ne(() => {
              }, ["stop"]))
            }, [
              s("div", null, [
                s("div", { class: "hidden sm:block absolute top-0 right-0 pt-4 pr-4 z-10" }, [
                  s("button", {
                    type: "button",
                    onClick: c,
                    class: "bg-white dark:bg-black rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black"
                  }, T1)
                ]),
                Z(g.$slots, "default")
              ])
            ], 34)
          ])
        ]),
        ((Y = h.value) == null ? void 0 : Y.name) == "ModalLookup" && h.value.ref ? (a(), se(U, {
          key: 0,
          "ref-info": h.value.ref,
          onDone: p
        }, null, 8, ["ref-info"])) : k("", !0)
      ], 40, L1);
    };
  }
}), I1 = {
  class: "pt-2 overflow-auto",
  style: { "min-height": "620px" }
}, D1 = { class: "mt-3 pl-5 flex flex-wrap items-center" }, O1 = { class: "hidden sm:block text-xl leading-6 font-medium text-gray-900 dark:text-gray-50 mr-3" }, j1 = { class: "hidden md:inline" }, P1 = { class: "flex pb-1 sm:pb-0" }, B1 = ["title"], R1 = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("g", {
    "stroke-width": "1.5",
    fill: "none"
  }, [
    /* @__PURE__ */ s("path", {
      d: "M9 3H3.6a.6.6 0 0 0-.6.6v16.8a.6.6 0 0 0 .6.6H9M9 3v18M9 3h6M9 21h6m0-18h5.4a.6.6 0 0 1 .6.6v16.8a.6.6 0 0 1-.6.6H15m0-18v18",
      stroke: "currentColor"
    })
  ])
], -1), H1 = [
  R1
], E1 = ["disabled"], z1 = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6l6 6zM6 6h2v12H6z",
    fill: "currentColor"
  })
], -1), N1 = [
  z1
], U1 = ["disabled"], q1 = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M15.41 7.41L14 6l-6 6l6 6l1.41-1.41L10.83 12z",
    fill: "currentColor"
  })
], -1), Q1 = [
  q1
], K1 = ["disabled"], Z1 = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M10 6L8.59 7.41L13.17 12l-4.58 4.59L10 18l6-6z",
    fill: "currentColor"
  })
], -1), G1 = [
  Z1
], W1 = ["disabled"], J1 = /* @__PURE__ */ s("svg", {
  class: "w-8 h-8",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    d: "M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6l-6-6zM16 6h2v12h-2z",
    fill: "currentColor"
  })
], -1), X1 = [
  J1
], Y1 = {
  key: 0,
  class: "flex pb-1 sm:pb-0"
}, em = { class: "px-4 text-lg text-black dark:text-white" }, tm = { key: 0 }, lm = { key: 1 }, nm = /* @__PURE__ */ s("span", { class: "hidden xl:inline" }, " Showing Results ", -1), sm = { key: 2 }, om = {
  key: 1,
  class: "pl-2"
}, am = /* @__PURE__ */ s("svg", {
  class: "w-5 h-5",
  xmlns: "http://www.w3.org/2000/svg",
  "aria-hidden": "true",
  viewBox: "0 0 24 24"
}, [
  /* @__PURE__ */ s("path", {
    fill: "currentColor",
    d: "M6.78 2.72a.75.75 0 0 1 0 1.06L4.56 6h8.69a7.75 7.75 0 1 1-7.75 7.75a.75.75 0 0 1 1.5 0a6.25 6.25 0 1 0 6.25-6.25H4.56l2.22 2.22a.75.75 0 1 1-1.06 1.06l-3.5-3.5a.75.75 0 0 1 0-1.06l3.5-3.5a.75.75 0 0 1 1.06 0Z"
  })
], -1), im = [
  am
], rm = { class: "flex pb-1 sm:pb-0" }, um = {
  key: 0,
  class: "pl-2"
}, dm = /* @__PURE__ */ s("svg", {
  class: "flex-none w-5 h-5 mr-2 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  "aria-hidden": "true",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor"
}, [
  /* @__PURE__ */ s("path", {
    "fill-rule": "evenodd",
    d: "M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z",
    "clip-rule": "evenodd"
  })
], -1), cm = { class: "mr-1" }, fm = {
  key: 0,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, mm = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z",
  "clip-rule": "evenodd"
}, null, -1), vm = [
  mm
], hm = {
  key: 1,
  class: "h-5 w-5 text-gray-400 dark:text-gray-500 group-hover:text-gray-500",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 20 20",
  fill: "currentColor",
  "aria-hidden": "true"
}, gm = /* @__PURE__ */ s("path", {
  "fill-rule": "evenodd",
  d: "M5 10a1 1 0 011-1h8a1 1 0 110 2H6a1 1 0 01-1-1z",
  "clip-rule": "evenodd"
}, null, -1), pm = [
  gm
], ym = { key: 1 }, bm = { key: 4 }, wm = { key: 0 }, xm = {
  key: 0,
  class: "cursor-pointer flex justify-between items-center hover:text-gray-900 dark:hover:text-gray-50"
}, km = { class: "mr-1 select-none" }, $m = {
  key: 1,
  class: "flex justify-between items-center"
}, Cm = { class: "mr-1 select-none" }, _m = /* @__PURE__ */ ue({
  __name: "ModalLookup",
  props: {
    id: { default: "ModalLookup" },
    refInfo: null,
    skip: { default: 0 },
    prefs: null,
    selectedColumns: null,
    allowFiltering: { type: [Boolean, null], default: !0 },
    showPreferences: { type: [Boolean, null], default: !0 },
    showPagingNav: { type: [Boolean, null], default: !0 },
    showPagingInfo: { type: [Boolean, null], default: !0 },
    showResetPreferences: { type: [Boolean, null], default: !0 },
    showFiltersView: { type: [Boolean, null], default: !0 },
    toolbarButtonClass: null,
    canFilter: null
  },
  emits: ["done"],
  setup(e, { emit: t }) {
    const l = e, n = Yl(), { config: i } = St(), { metadataApi: r, filterDefinitions: d } = ot(), c = qe("client"), m = i.value.storage, h = f(() => l.toolbarButtonClass ?? he.toolbarButtonClass), y = f(() => d.value), b = 25, p = M({ take: b }), v = M(new We()), g = M(l.skip), O = M(!1), U = M(), Y = (z) => typeof z == "string" ? z.split(",") : z || [];
    function R(z, Q) {
      return he.getTableRowClass("fullWidth", Q, !1, !0);
    }
    function N() {
      let z = Y(l.selectedColumns);
      return z.length > 0 ? z : [];
    }
    const T = f(() => st(l.refInfo.model)), J = f(() => {
      let Q = N().map((ge) => ge.toLowerCase());
      const oe = Je(T.value);
      return Q.length > 0 ? Q.map((ge) => oe.find((Se) => Se.name.toLowerCase() === ge)).filter((ge) => ge != null) : oe;
    }), j = f(() => {
      let z = J.value.map((oe) => oe.name), Q = Y(p.value.selectedColumns).map((oe) => oe.toLowerCase());
      return Q.length > 0 ? z.filter((oe) => Q.includes(oe.toLowerCase())) : z;
    }), L = f(() => p.value.take ?? b), q = f(() => v.value.response ? be(v.value.response, "results") : []), D = f(() => {
      var z;
      return ((z = v.value.response) == null ? void 0 : z.total) ?? q.value.length ?? 0;
    }), K = f(() => g.value > 0), ne = f(() => g.value > 0), ee = f(() => q.value.length >= L.value), te = f(() => q.value.length >= L.value), S = M([]), le = f(() => S.value.some((z) => z.settings.filters.length > 0 || !!z.settings.sort)), x = f(() => S.value.map((z) => z.settings.filters.length).reduce((z, Q) => z + Q, 0)), W = f(() => el(T.value)), E = f(() => {
      var z;
      return (z = r.value) == null ? void 0 : z.operations.find((Q) => {
        var oe;
        return ((oe = Q.dataModel) == null ? void 0 : oe.name) == l.refInfo.model && ze.isAnyQuery(Q);
      });
    }), G = M(), C = M(!1), _ = M(), B = () => `${l.id}/ApiPrefs/${l.refInfo.model}`, de = (z) => `Column/${l.id}:${l.refInfo.model}.${z}`;
    async function F(z) {
      g.value += z, g.value < 0 && (g.value = 0);
      var Q = Math.floor(D.value / L.value) * L.value;
      g.value > Q && (g.value = Q), await Ce();
    }
    async function V(z, Q) {
      t("done", z);
    }
    function re() {
      t("done", null);
    }
    function ye(z, Q) {
      var ge, Se, Oe;
      let oe = Q.target;
      if ((oe == null ? void 0 : oe.tagName) !== "TD") {
        let Re = (ge = oe == null ? void 0 : oe.closest("TABLE")) == null ? void 0 : ge.getBoundingClientRect(), Me = S.value.find((De) => De.name.toLowerCase() == z.toLowerCase());
        if (Me && Re) {
          let De = 318, je = (((Se = Q.target) == null ? void 0 : Se.tagName) === "DIV" ? Q.target : (Oe = Q.target) == null ? void 0 : Oe.closest("DIV")).getBoundingClientRect(), Ye = De + 25;
          _.value = {
            column: Me,
            topLeft: {
              x: Math.max(Math.floor(je.x + 25), Ye),
              y: Math.floor(115)
            }
          };
        }
      }
    }
    function fe() {
      _.value = null;
    }
    async function H(z) {
      var oe;
      let Q = (oe = _.value) == null ? void 0 : oe.column;
      Q && (Q.settings = z, m.setItem(de(Q.name), JSON.stringify(Q.settings)), await Ce()), _.value = null;
    }
    async function P(z) {
      m.setItem(de(z.name), JSON.stringify(z.settings)), await Ce();
    }
    async function me(z) {
      C.value = !1, p.value = z, m.setItem(B(), JSON.stringify(z)), await Ce();
    }
    async function Ce() {
      await ve(Ve());
    }
    async function ve(z) {
      const Q = E.value;
      if (!Q) {
        console.error(`No Query API was found for ${l.refInfo.model}`);
        return;
      }
      let oe = Gt(Q, z), ge = Wn((Re) => {
        v.value.response = v.value.error = void 0, O.value = Re;
      }), Se = await c.api(oe);
      ge(), xt(() => v.value = Se);
      let Oe = be(Se.response, "results") || [];
      !Se.succeeded || Oe.label == 0;
    }
    function Ve() {
      let z = {
        include: "total",
        take: L.value
      }, Q = Y(p.value.selectedColumns || l.selectedColumns);
      if (Q.length > 0) {
        let ge = W.value;
        ge && Q.includes(ge.name) && (Q = [ge.name, ...Q]), z.fields = Q.join(",");
      }
      let oe = [];
      return S.value.forEach((ge) => {
        ge.settings.sort && oe.push((ge.settings.sort === "DESC" ? "-" : "") + ge.name), ge.settings.filters.forEach((Se) => {
          let Oe = Se.key.replace("%", ge.name);
          z[Oe] = Se.value;
        });
      }), typeof z.skip > "u" && g.value > 0 && (z.skip = g.value), oe.length > 0 && (z.orderBy = oe.join(",")), z;
    }
    async function Fe() {
      S.value.forEach((z) => {
        z.settings = { filters: [] }, m.removeItem(de(z.name));
      }), await Ce();
    }
    return Xe(async () => {
      const z = l.prefs || yl(m.getItem(B()));
      z && (p.value = z), S.value = J.value.map((Q) => ({
        name: Q.name,
        type: Q.type,
        meta: Q,
        settings: Object.assign(
          {
            filters: []
          },
          yl(m.getItem(de(Q.name)))
        )
      })), isNaN(l.skip) || (g.value = l.skip), await Ce();
    }), (z, Q) => {
      const oe = X("ErrorSummary"), ge = X("Loading"), Se = X("SettingsIcons"), Oe = X("DataGrid"), Re = X("ModalDialog");
      return a(), u(Te, null, [
        e.refInfo ? (a(), se(Re, {
          key: 0,
          ref_key: "modalDialog",
          ref: G,
          id: e.id,
          onDone: re
        }, {
          default: ke(() => [
            s("div", I1, [
              s("div", D1, [
                s("h3", O1, [
                  xe(" Select "),
                  s("span", j1, A(o(Ee)(e.refInfo.model)), 1)
                ]),
                s("div", P1, [
                  e.showPreferences ? (a(), u("button", {
                    key: 0,
                    type: "button",
                    class: "pl-2 text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400",
                    title: `${e.refInfo.model} Preferences`,
                    onClick: Q[0] || (Q[0] = (Me) => C.value = !C.value)
                  }, H1, 8, B1)) : k("", !0),
                  e.showPagingNav ? (a(), u("button", {
                    key: 1,
                    type: "button",
                    class: w(["pl-2", o(K) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "First page",
                    disabled: !o(K),
                    onClick: Q[1] || (Q[1] = (Me) => F(-o(D)))
                  }, N1, 10, E1)) : k("", !0),
                  e.showPagingNav ? (a(), u("button", {
                    key: 2,
                    type: "button",
                    class: w(["pl-2", o(ne) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Previous page",
                    disabled: !o(ne),
                    onClick: Q[2] || (Q[2] = (Me) => F(-o(L)))
                  }, Q1, 10, U1)) : k("", !0),
                  e.showPagingNav ? (a(), u("button", {
                    key: 3,
                    type: "button",
                    class: w(["pl-2", o(ee) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Next page",
                    disabled: !o(ee),
                    onClick: Q[3] || (Q[3] = (Me) => F(o(L)))
                  }, G1, 10, K1)) : k("", !0),
                  e.showPagingNav ? (a(), u("button", {
                    key: 4,
                    type: "button",
                    class: w(["pl-2", o(te) ? "text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400" : "text-gray-400 dark:text-gray-500"]),
                    title: "Last page",
                    disabled: !o(te),
                    onClick: Q[4] || (Q[4] = (Me) => F(o(D)))
                  }, X1, 10, W1)) : k("", !0)
                ]),
                e.showPagingInfo ? (a(), u("div", Y1, [
                  s("div", em, [
                    O.value ? (a(), u("span", tm, "Querying...")) : k("", !0),
                    o(q).length ? (a(), u("span", lm, [
                      nm,
                      xe(" " + A(g.value + 1) + " - " + A(Math.min(g.value + o(q).length, o(D))) + " ", 1),
                      s("span", null, " of " + A(o(D)), 1)
                    ])) : v.value.completed ? (a(), u("span", sm, "No Results")) : k("", !0)
                  ])
                ])) : k("", !0),
                o(le) && e.showResetPreferences ? (a(), u("div", om, [
                  s("button", {
                    type: "button",
                    onClick: Fe,
                    title: "Reset Preferences & Filters",
                    class: w(o(h))
                  }, im, 2)
                ])) : k("", !0),
                s("div", rm, [
                  e.showFiltersView && o(x) > 0 ? (a(), u("div", um, [
                    s("button", {
                      type: "button",
                      onClick: Q[5] || (Q[5] = (Me) => U.value = U.value == "filters" ? null : "filters"),
                      class: w(o(h)),
                      "aria-expanded": "false"
                    }, [
                      dm,
                      s("span", cm, A(o(x)) + " " + A(o(x) == 1 ? "Filter" : "Filters"), 1),
                      U.value != "filters" ? (a(), u("svg", fm, vm)) : (a(), u("svg", hm, pm))
                    ], 2)
                  ])) : k("", !0)
                ])
              ]),
              U.value == "filters" ? (a(), se(Cn, {
                key: 0,
                class: "border-y border-gray-200 dark:border-gray-800 py-8 my-2",
                definitions: o(y),
                columns: S.value,
                onDone: Q[6] || (Q[6] = (Me) => U.value = null),
                onChange: P
              }, null, 8, ["definitions", "columns"])) : k("", !0),
              _.value ? (a(), u("div", ym, [
                $e($n, {
                  definitions: o(y),
                  column: _.value.column,
                  "top-left": _.value.topLeft,
                  onDone: fe,
                  onSave: H
                }, null, 8, ["definitions", "column", "top-left"])
              ])) : k("", !0),
              v.value.error ? (a(), se(oe, {
                key: 2,
                status: v.value.error
              }, null, 8, ["status"])) : O.value ? (a(), se(ge, { key: 3 })) : (a(), u("div", bm, [
                o(q).length ? (a(), u("div", wm, [
                  $e(Oe, {
                    id: e.id,
                    items: o(q),
                    type: e.refInfo.model,
                    "selected-columns": o(j),
                    onFiltersChanged: Ce,
                    tableStyle: "fullWidth",
                    rowClass: R,
                    onRowSelected: V,
                    onHeaderSelected: ye
                  }, en({
                    header: ke(({ column: Me, label: De }) => {
                      var Qe;
                      return [
                        e.allowFiltering && (!l.canFilter || l.canFilter(Me)) ? (a(), u("div", xm, [
                          s("span", km, A(De), 1),
                          $e(Se, {
                            column: S.value.find((je) => je.name.toLowerCase() === Me.toLowerCase()),
                            "is-open": ((Qe = _.value) == null ? void 0 : Qe.column.name) === Me
                          }, null, 8, ["column", "is-open"])
                        ])) : (a(), u("div", $m, [
                          s("span", Cm, A(De), 1)
                        ]))
                      ];
                    }),
                    _: 2
                  }, [
                    Ie(Object.keys(o(n)), (Me) => ({
                      name: Me,
                      fn: ke((De) => [
                        Z(z.$slots, Me, It(gl(De)))
                      ])
                    }))
                  ]), 1032, ["id", "items", "type", "selected-columns"])
                ])) : k("", !0)
              ]))
            ])
          ]),
          _: 3
        }, 8, ["id"])) : k("", !0),
        C.value ? (a(), se(_n, {
          key: 1,
          columns: o(J),
          prefs: p.value,
          onDone: Q[7] || (Q[7] = (Me) => C.value = !1),
          onSave: me
        }, null, 8, ["columns", "prefs"])) : k("", !0)
      ], 64);
    };
  }
}), Lm = { class: "sm:hidden" }, Vm = ["for"], Sm = ["id", "name"], Mm = ["value"], Am = { class: "hidden sm:block" }, Tm = { class: "border-b border-gray-200" }, Fm = {
  class: "-mb-px flex",
  "aria-label": "Tabs"
}, Im = ["onClick"], Dm = /* @__PURE__ */ ue({
  __name: "Tabs",
  props: {
    tabs: null,
    id: { default: "tabs" },
    param: { default: "tab" },
    label: { type: Function, default: (e) => Ee(e) },
    selected: null,
    tabClass: null,
    bodyClass: { default: "p-4" },
    url: { type: Boolean, default: !0 }
  },
  setup(e) {
    const t = e, l = f(() => Object.keys(t.tabs)), n = (y) => t.label ? t.label(y) : Ee(y), i = f(() => t.id || "tabs"), r = f(() => t.param || "tab"), d = M();
    function c(y) {
      if (d.value = y, t.url) {
        const b = l.value[0];
        ln({ tab: y === b ? void 0 : y });
      }
    }
    function m(y) {
      return d.value === y;
    }
    const h = f(() => `${100 / Object.keys(t.tabs).length}%`);
    return Xe(() => {
      if (d.value = t.selected || Object.keys(t.tabs)[0], t.url) {
        const y = location.search ? location.search : location.hash.includes("?") ? "?" + dl(location.hash, "?") : "", p = El(y)[r.value];
        p && (d.value = p);
      }
    }), (y, b) => (a(), u("div", null, [
      s("div", Lm, [
        s("label", {
          for: o(i),
          class: "sr-only"
        }, "Select a tab", 8, Vm),
        s("select", {
          id: o(i),
          name: o(i),
          class: "block w-full rounded-md border-gray-300 focus:border-indigo-500 focus:ring-indigo-500",
          onChange: b[0] || (b[0] = (p) => {
            var v;
            return c((v = p.target) == null ? void 0 : v.value);
          })
        }, [
          (a(!0), u(Te, null, Ie(o(l), (p) => (a(), u("option", {
            key: p,
            value: p
          }, A(n(p)), 9, Mm))), 128))
        ], 40, Sm)
      ]),
      s("div", Am, [
        s("div", Tm, [
          s("nav", Fm, [
            (a(!0), u(Te, null, Ie(o(l), (p) => (a(), u("a", {
              href: "#",
              onClick: Ne((v) => c(p), ["prevent"]),
              style: Jl({ width: o(h) }),
              class: w([m(p) ? "border-indigo-500 text-indigo-600 py-4 px-1 text-center border-b-2 font-medium text-sm" : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 py-4 px-1 text-center border-b-2 font-medium text-sm", e.tabClass])
            }, A(n(p)), 15, Im))), 256))
          ])
        ])
      ]),
      s("div", {
        class: w(e.bodyClass)
      }, [
        (a(), se(Kn(e.tabs[d.value])))
      ], 2)
    ]));
  }
}), Om = /* @__PURE__ */ s("svg", {
  xmlns: "http://www.w3.org/2000/svg",
  class: "h-4 w-4 text-gray-400",
  preserveAspectRatio: "xMidYMid meet",
  viewBox: "0 0 32 32"
}, [
  /* @__PURE__ */ s("path", {
    fill: "currentColor",
    d: "M13.502 5.414a15.075 15.075 0 0 0 11.594 18.194a11.113 11.113 0 0 1-7.975 3.39c-.138 0-.278.005-.418 0a11.094 11.094 0 0 1-3.2-21.584M14.98 3a1.002 1.002 0 0 0-.175.016a13.096 13.096 0 0 0 1.825 25.981c.164.006.328 0 .49 0a13.072 13.072 0 0 0 10.703-5.555a1.01 1.01 0 0 0-.783-1.565A13.08 13.08 0 0 1 15.89 4.38A1.015 1.015 0 0 0 14.98 3Z"
  })
], -1), jm = [
  Om
], Pm = /* @__PURE__ */ s("svg", {
  xmlns: "http://www.w3.org/2000/svg",
  class: "h-4 w-4 text-indigo-600",
  preserveAspectRatio: "xMidYMid meet",
  viewBox: "0 0 32 32"
}, [
  /* @__PURE__ */ s("path", {
    fill: "currentColor",
    d: "M16 12.005a4 4 0 1 1-4 4a4.005 4.005 0 0 1 4-4m0-2a6 6 0 1 0 6 6a6 6 0 0 0-6-6ZM5.394 6.813L6.81 5.399l3.505 3.506L8.9 10.319zM2 15.005h5v2H2zm3.394 10.193L8.9 21.692l1.414 1.414l-3.505 3.506zM15 25.005h2v5h-2zm6.687-1.9l1.414-1.414l3.506 3.506l-1.414 1.414zm3.313-8.1h5v2h-5zm-3.313-6.101l3.506-3.506l1.414 1.414l-3.506 3.506zM15 2.005h2v5h-2z"
  })
], -1), Bm = [
  Pm
], Rm = /* @__PURE__ */ ue({
  __name: "DarkModeToggle",
  setup(e) {
    const t = typeof document < "u" ? document.querySelector("html") : null;
    let l = M(t == null ? void 0 : t.classList.contains("dark"));
    function n() {
      l.value ? t == null || t.classList.remove("dark") : t == null || t.classList.add("dark"), l.value = !l.value, localStorage.setItem("color-scheme", l.value ? "dark" : "light");
    }
    return (i, r) => (a(), u("button", {
      type: "button",
      class: "bg-gray-200 dark:bg-gray-700 relative inline-flex flex-shrink-0 h-6 w-11 border-2 border-transparent rounded-full cursor-pointer transition-colors ease-in-out duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:ring-offset-black",
      role: "switch",
      "aria-checked": "false",
      onClick: r[0] || (r[0] = (d) => n())
    }, [
      s("span", {
        class: w(`${o(l) ? "translate-x-0" : "translate-x-5"} pointer-events-none relative inline-block h-5 w-5 rounded-full bg-white dark:bg-black shadow transform ring-0 transition ease-in-out duration-200`)
      }, [
        s("span", {
          class: w(`${o(l) ? "opacity-100 ease-in duration-200" : "opacity-0 ease-out duration-100"} absolute inset-0 h-full w-full flex items-center justify-center transition-opacity`),
          "aria-hidden": "true"
        }, jm, 2),
        s("span", {
          class: w(`${o(l) ? "opacity-0 ease-out duration-100" : "opacity-100 ease-in duration-200"} absolute inset-0 h-full w-full flex items-center justify-center transition-opacity`),
          "aria-hidden": "true"
        }, Bm, 2)
      ], 2)
    ]));
  }
}), Hm = { key: 0 }, Em = {
  key: 1,
  class: "min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8"
}, zm = { class: "sm:mx-auto sm:w-full sm:max-w-md" }, Nm = { class: "mt-6 text-center text-3xl font-extrabold text-gray-900" }, Um = {
  key: 0,
  class: "mt-4 text-center text-sm text-gray-600"
}, qm = { class: "relative z-0 inline-flex shadow-sm rounded-md" }, Qm = ["onClick"], Km = { class: "mt-8 sm:mx-auto sm:w-full sm:max-w-md" }, Zm = { class: "bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10" }, Gm = ["onSubmit"], Wm = { class: "mt-8" }, Jm = {
  key: 1,
  class: "mt-6"
}, Xm = /* @__PURE__ */ wl('<div class="relative"><div class="absolute inset-0 flex items-center"><div class="w-full border-t border-gray-300"></div></div><div class="relative flex justify-center text-sm"><span class="px-2 bg-white text-gray-500"> Or continue with </span></div></div>', 1), Ym = { class: "mt-6 grid grid-cols-3 gap-3" }, ev = ["href", "title"], tv = {
  key: 1,
  class: "h-5 w-5 text-gray-700",
  xmlns: "http://www.w3.org/2000/svg",
  viewBox: "0 0 32 32"
}, lv = /* @__PURE__ */ s("path", {
  d: "M16 8a5 5 0 1 0 5 5a5 5 0 0 0-5-5z",
  fill: "currentColor"
}, null, -1), nv = /* @__PURE__ */ s("path", {
  d: "M16 2a14 14 0 1 0 14 14A14.016 14.016 0 0 0 16 2zm7.992 22.926A5.002 5.002 0 0 0 19 20h-6a5.002 5.002 0 0 0-4.992 4.926a12 12 0 1 1 15.985 0z",
  fill: "currentColor"
}, null, -1), sv = [
  lv,
  nv
], ov = /* @__PURE__ */ ue({
  __name: "SignIn",
  props: {
    provider: null,
    title: { default: "Sign In" },
    tabs: { type: [Boolean, String], default: !0 },
    oauth: { type: [Boolean, String], default: !0 }
  },
  emits: ["login"],
  setup(e, { emit: t }) {
    const l = e, { getMetadata: n, createDto: i } = ot(), r = Tl(), { signIn: d } = kn(), c = n({ assert: !0 }), m = c.plugins.auth, h = document.baseURI, y = c.app.baseUrl, b = M(i("Authenticate")), p = M(new We()), v = M(l.provider);
    Xe(() => {
      m == null || m.authProviders.map((D) => D.formLayout).filter((D) => D).forEach((D) => D.forEach((K) => b.value[K.id] = ""));
    });
    const g = f(() => (m == null ? void 0 : m.authProviders.filter((D) => D.formLayout)) || []), O = f(() => g.value[0] || {}), U = f(() => g.value[Math.max(g.value.length - 1, 0)] || {}), Y = f(() => (v.value ? m == null ? void 0 : m.authProviders.find((D) => D.name === v.value) : null) ?? O.value), R = (D) => D === !1 || D === "false";
    function N(D) {
      return D.label || D.navItem && D.navItem.label;
    }
    const T = f(() => {
      var D;
      return (((D = Y.value) == null ? void 0 : D.formLayout) || []).map((K) => {
        var ne, ee;
        return Object.assign({}, K, {
          type: (ne = K.type) == null ? void 0 : ne.toLowerCase(),
          autocomplete: K.autocomplete || (((ee = K.type) == null ? void 0 : ee.toLowerCase()) === "password" ? "current-password" : void 0) || (K.id.toLowerCase() === "username" ? "username" : void 0),
          css: Object.assign({ field: "col-span-12" }, K.css)
        });
      });
    }), J = f(() => R(l.oauth) ? [] : (m == null ? void 0 : m.authProviders.filter((D) => D.type === "oauth")) || []), j = f(() => {
      let D = xo(
        m == null ? void 0 : m.authProviders.filter((ne) => ne.formLayout && ne.formLayout.length > 0),
        (ne, ee) => {
          let te = N(ee) || nt(ee.name);
          ne[te] = ee.name === O.value.name ? "" : ee.name;
        }
      );
      const K = Y.value;
      return K && R(l.tabs) && (D = { [N(K) || nt(K.name)]: K }), D;
    }), L = f(() => {
      let D = T.value.map((K) => K.id).filter((K) => K);
      return p.value.summaryMessage(D);
    });
    async function q() {
      if (b.value.provider = Y.value.name, p.value = await r.api(b.value), p.value.succeeded) {
        const D = p.value.response;
        d(D), t("login", D), p.value = new We(), b.value = i("Authenticate");
      }
    }
    return (D, K) => {
      const ne = X("ErrorSummary"), ee = X("AutoFormFields"), te = X("PrimaryButton"), S = X("Icon"), le = oo("href");
      return o(m) ? (a(), u("div", Em, [
        s("div", zm, [
          s("h2", Nm, A(e.title), 1),
          Object.keys(o(j)).length > 1 ? (a(), u("p", Um, [
            s("span", qm, [
              (a(!0), u(Te, null, Ie(o(j), (x, W) => kt((a(), u("a", {
                onClick: (E) => v.value = x,
                class: w([
                  x === "" || x === o(O).name ? "rounded-l-md" : x === o(U).name ? "rounded-r-md -ml-px" : "-ml-px",
                  v.value === x ? "z-10 outline-none ring-1 ring-indigo-500 border-indigo-500" : "",
                  "cursor-pointer relative inline-flex items-center px-4 py-1 border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50"
                ])
              }, [
                xe(A(W), 1)
              ], 10, Qm)), [
                [le, { provider: x }]
              ])), 256))
            ])
          ])) : k("", !0)
        ]),
        s("div", Km, [
          o(L) ? (a(), se(ne, {
            key: 0,
            class: "mb-3",
            errorSummary: o(L)
          }, null, 8, ["errorSummary"])) : k("", !0),
          s("div", Zm, [
            o(T).length ? (a(), u("form", {
              key: 0,
              onSubmit: Ne(q, ["prevent"])
            }, [
              $e(ee, {
                modelValue: b.value,
                formLayout: o(T),
                api: p.value,
                hideSummary: !0,
                "divide-class": "",
                "space-class": "space-y-6"
              }, null, 8, ["modelValue", "formLayout", "api"]),
              s("div", Wm, [
                $e(te, { class: "w-full" }, {
                  default: ke(() => [
                    xe("Sign In")
                  ]),
                  _: 1
                })
              ])
            ], 40, Gm)) : k("", !0),
            o(J).length ? (a(), u("div", Jm, [
              Xm,
              s("div", Ym, [
                (a(!0), u(Te, null, Ie(o(J), (x) => (a(), u("div", null, [
                  s("a", {
                    href: o(y) + x.navItem.href + "?continue=" + o(h),
                    title: N(x),
                    class: "w-full inline-flex justify-center py-2 px-4 border border-gray-300 rounded-md shadow-sm bg-white text-sm font-medium text-gray-500 hover:bg-gray-50"
                  }, [
                    x.icon ? (a(), se(S, {
                      key: 0,
                      image: x.icon,
                      class: "h-5 w-5 text-gray-700"
                    }, null, 8, ["image"])) : (a(), u("svg", tv, sv))
                  ], 8, ev)
                ]))), 256))
              ])
            ])) : k("", !0)
          ])
        ])
      ])) : (a(), u("div", Hm, "No Auth Plugin"));
    };
  }
}), av = ["for"], iv = {
  key: 1,
  class: "border border-gray-200 flex justify-between"
}, rv = { class: "p-2 flex flex-wrap gap-x-4" }, uv = /* @__PURE__ */ s("title", null, "Bold text (CTRL+B)", -1), dv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M15.6 10.79c.97-.67 1.65-1.77 1.65-2.79c0-2.26-1.75-4-4-4H7v14h7.04c2.09 0 3.71-1.7 3.71-3.79c0-1.52-.86-2.82-2.15-3.42zM10 6.5h3c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5h-3v-3zm3.5 9H10v-3h3.5c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5z"
}, null, -1), cv = [
  uv,
  dv
], fv = /* @__PURE__ */ s("title", null, "Italics (CTRL+I)", -1), mv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M10 4v3h2.21l-3.42 8H6v3h8v-3h-2.21l3.42-8H18V4h-8z"
}, null, -1), vv = [
  fv,
  mv
], hv = /* @__PURE__ */ s("title", null, "Insert Link (CTRL+K)", -1), gv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7a5 5 0 0 0-5 5a5 5 0 0 0 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1M8 13h8v-2H8v2m9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1c0 1.71-1.39 3.1-3.1 3.1h-4V17h4a5 5 0 0 0 5-5a5 5 0 0 0-5-5Z"
}, null, -1), pv = [
  hv,
  gv
], yv = /* @__PURE__ */ s("title", null, "Blockquote (CTRL+Q)", -1), bv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "m15 17l2-4h-4V6h7v7l-2 4h-3Zm-9 0l2-4H4V6h7v7l-2 4H6Z"
}, null, -1), wv = [
  yv,
  bv
], xv = /* @__PURE__ */ s("title", null, "Insert Image (CTRL+SHIFT+L)", -1), kv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M2.992 21A.993.993 0 0 1 2 20.007V3.993A1 1 0 0 1 2.992 3h18.016c.548 0 .992.445.992.993v16.014a1 1 0 0 1-.992.993H2.992ZM20 15V5H4v14L14 9l6 6Zm0 2.828l-6-6L6.828 19H20v-1.172ZM8 11a2 2 0 1 1 0-4a2 2 0 0 1 0 4Z"
}, null, -1), $v = [
  xv,
  kv
], Cv = /* @__PURE__ */ s("title", null, "Insert Code (CTRL+<)", -1), _v = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "m8 18l-6-6l6-6l1.425 1.425l-4.6 4.6L9.4 16.6L8 18Zm8 0l-1.425-1.425l4.6-4.6L14.6 7.4L16 6l6 6l-6 6Z"
}, null, -1), Lv = [
  Cv,
  _v
], Vv = /* @__PURE__ */ s("title", null, "H2 Heading (CTRL+H)", -1), Sv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M7 20V7H2V4h13v3h-5v13H7Zm9 0v-8h-3V9h9v3h-3v8h-3Z"
}, null, -1), Mv = [
  Vv,
  Sv
], Av = /* @__PURE__ */ s("title", null, "Numbered List (ALT+1)", -1), Tv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M3 22v-1.5h2.5v-.75H4v-1.5h1.5v-.75H3V16h3q.425 0 .713.288T7 17v1q0 .425-.288.713T6 19q.425 0 .713.288T7 20v1q0 .425-.288.713T6 22H3Zm0-7v-2.75q0-.425.288-.713T4 11.25h1.5v-.75H3V9h3q.425 0 .713.288T7 10v1.75q0 .425-.288.713T6 12.75H4.5v.75H7V15H3Zm1.5-7V3.5H3V2h3v6H4.5ZM9 19v-2h12v2H9Zm0-6v-2h12v2H9Zm0-6V5h12v2H9Z"
}, null, -1), Fv = [
  Av,
  Tv
], Iv = /* @__PURE__ */ s("title", null, "Bulleted List (ALT+-)", -1), Dv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M9 19v-2h12v2H9Zm0-6v-2h12v2H9Zm0-6V5h12v2H9ZM5 20q-.825 0-1.413-.588T3 18q0-.825.588-1.413T5 16q.825 0 1.413.588T7 18q0 .825-.588 1.413T5 20Zm0-6q-.825 0-1.413-.588T3 12q0-.825.588-1.413T5 10q.825 0 1.413.588T7 12q0 .825-.588 1.413T5 14Zm0-6q-.825 0-1.413-.588T3 6q0-.825.588-1.413T5 4q.825 0 1.413.588T7 6q0 .825-.588 1.413T5 8Z"
}, null, -1), Ov = [
  Iv,
  Dv
], jv = /* @__PURE__ */ s("title", null, "Strike Through (ALT+S)", -1), Pv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M10 19h4v-3h-4v3zM5 4v3h5v3h4V7h5V4H5zM3 14h18v-2H3v2z"
}, null, -1), Bv = [
  jv,
  Pv
], Rv = /* @__PURE__ */ s("title", null, "Undo (CTRL+Z)", -1), Hv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M12.5 8c-2.65 0-5.05.99-6.9 2.6L2 7v9h9l-3.62-3.62c1.39-1.16 3.16-1.88 5.12-1.88c3.54 0 6.55 2.31 7.6 5.5l2.37-.78C21.08 11.03 17.15 8 12.5 8z"
}, null, -1), Ev = [
  Rv,
  Hv
], zv = /* @__PURE__ */ s("title", null, "Redo (CTRL+SHIFT+Z)", -1), Nv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M18.4 10.6C16.55 8.99 14.15 8 11.5 8c-4.65 0-8.58 3.03-9.96 7.22L3.9 16a8.002 8.002 0 0 1 7.6-5.5c1.95 0 3.73.72 5.12 1.88L13 16h9V7l-3.6 3.6z"
}, null, -1), Uv = [
  zv,
  Nv
], qv = {
  key: 0,
  class: "p-2 flex flex-wrap gap-x-4"
}, Qv = ["href"], Kv = /* @__PURE__ */ s("path", {
  fill: "currentColor",
  d: "M11 18h2v-2h-2v2zm1-16C6.48 2 2 6.48 2 12s4.48 10 10 10s10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8s8 3.59 8 8s-3.59 8-8 8zm0-14c-2.21 0-4 1.79-4 4h2c0-1.1.9-2 2-2s2 .9 2 2c0 2-3 1.75-3 5h2c0-2.25 3-2.5 3-5c0-2.21-1.79-4-4-4z"
}, null, -1), Zv = [
  Kv
], Gv = { class: "" }, Wv = ["name", "id", "label", "value", "rows", "disabled", "onKeydown"], Jv = ["id"], Xv = ["id"], Yv = /* @__PURE__ */ ue({
  __name: "MarkdownInput",
  props: {
    status: null,
    id: null,
    inputClass: null,
    label: null,
    labelClass: null,
    help: null,
    placeholder: null,
    modelValue: null,
    counter: { type: Boolean },
    rows: null,
    errorMessages: null,
    lang: null,
    autoFocus: { type: Boolean },
    disabled: { type: Boolean },
    helpUrl: { default: "https://guides.github.com/features/mastering-markdown/" },
    hide: null
  },
  emits: ["update:modelValue", "close"],
  setup(e, { expose: t, emit: l }) {
    const n = e;
    let i = [], r = [], d = qe("ApiState", void 0);
    const c = f(() => ft.call({ responseStatus: n.status ?? (d == null ? void 0 : d.error.value) }, n.id)), m = f(() => n.label ?? Ee(nt(n.id))), h = "bold,italics,link,image,blockquote,code,heading,orderedList,unorderedList,strikethrough,undo,redo,help".split(","), y = f(() => n.hide ? bt(h, n.hide) : bt(h, []));
    function b(C) {
      return y.value[C];
    }
    const p = f(() => ["shadow-sm font-mono" + lt.base.replace("rounded-md", ""), c.value ? "text-red-900 focus:ring-red-500 focus:border-red-500 border-red-300" : "text-gray-900 " + lt.valid, n.inputClass]), v = "w-5 h-5 cursor-pointer select-none text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400", g = M();
    t({ props: n, textarea: g, updateModelValue: O, selection: Y, hasSelection: U, selectionInfo: R, insert: T, replace: N });
    function O(C) {
      l("update:modelValue", C);
    }
    function U() {
      return g.value.selectionStart !== g.value.selectionEnd;
    }
    function Y() {
      const C = g.value;
      return C.value.substring(C.selectionStart, C.selectionEnd) || "";
    }
    function R() {
      const C = g.value, _ = C.value, B = C.selectionStart, de = _.substring(B, C.selectionEnd) || "", F = _.substring(0, B), V = F.lastIndexOf(`
`);
      return {
        value: _,
        sel: de,
        selPos: B,
        beforeSel: F,
        afterSel: _.substring(B),
        prevCRPos: V,
        beforeCR: V >= 0 ? F.substring(0, V + 1) : "",
        afterCR: V >= 0 ? F.substring(V + 1) : ""
      };
    }
    function N({ value: C, selectionStart: _, selectionEnd: B }) {
      B == null && (B = _), O(C), xt(() => {
        g.value.focus(), g.value.setSelectionRange(_, B);
      });
    }
    function T(C, _, B = "", { selectionAtEnd: de, offsetStart: F, offsetEnd: V, filterValue: re, filterSelection: ye } = {}) {
      const fe = g.value;
      let H = fe.value, P = fe.selectionEnd;
      i.push({ value: H, selectionStart: fe.selectionStart, selectionEnd: fe.selectionEnd }), r = [];
      const me = fe.selectionStart, Ce = fe.selectionEnd;
      let ve = H.substring(0, me), Ve = H.substring(Ce);
      const Fe = C && ve.endsWith(C) && Ve.startsWith(_);
      if (me == Ce) {
        if (Fe ? (H = ve.substring(0, ve.length - C.length) + Ve.substring(_.length), P += -_.length) : (H = ve + C + B + _ + Ve, P += C.length, F = 0, V = (B == null ? void 0 : B.length) || 0, de && (P += V, V = 0)), re) {
          var Q = { pos: P };
          H = re(H, Q), P = Q.pos;
        }
      } else {
        var oe = H.substring(me, Ce);
        ye && (oe = ye(oe)), Fe ? (H = ve.substring(0, ve.length - C.length) + oe + Ve.substring(_.length), F = -oe.length - C.length, V = oe.length) : (H = ve + C + oe + _ + Ve, F ? P += (C + _).length : (P = me, F = C.length, V = oe.length));
      }
      O(H), xt(() => {
        fe.focus(), F = P + (F || 0), V = (F || 0) + (V || 0), fe.setSelectionRange(F, V);
      });
    }
    const J = () => T("**", "**", "bold"), j = () => T("_", "_", "italics"), L = () => T("~~", "~~", "strikethrough"), q = () => T("[", "](https://)", "", { offsetStart: -9, offsetEnd: 8 }), D = () => T(`
> `, `
`, "Blockquote", {}), K = () => T("![](", ")");
    function ne(C) {
      const _ = Y();
      if (_ && !C.shiftKey)
        T("`", "`", "code");
      else {
        const B = n.lang || "js";
        _.indexOf(`
`) === -1 ? T("\n```" + B + `
`, "\n```\n", "// code") : T("```" + B + `
`, "```\n", "");
      }
    }
    function ee() {
      if (U()) {
        let { sel: C, selPos: _, beforeSel: B, afterSel: de, prevCRPos: F, beforeCR: V, afterCR: re } = R();
        if (C.indexOf(`
`) === -1)
          T(`
 1. `, `
`);
        else if (!C.startsWith(" 1. ")) {
          let H = 1;
          T("", "", " - ", {
            selectionAtEnd: !0,
            filterSelection: (P) => " 1. " + P.replace(/\n$/, "").replace(/\n/g, (me) => `
 ${++H}. `) + `
`
          });
        } else
          T("", "", "", {
            filterValue: (H, P) => {
              if (F >= 0) {
                let me = re.replace(/^ - /, "");
                B = V + me, P.pos -= re.length - me.length;
              }
              return B + de;
            },
            filterSelection: (H) => H.replace(/^ 1. /g, "").replace(/\n \d+. /g, `
`)
          });
      } else
        T(`
 1. `, `
`, "List Item", { offsetStart: -10, offsetEnd: 9 });
    }
    function te() {
      if (U()) {
        let { sel: C, selPos: _, beforeSel: B, afterSel: de, prevCRPos: F, beforeCR: V, afterCR: re } = R();
        C.indexOf(`
`) === -1 ? T(`
 - `, `
`) : !C.startsWith(" - ") ? T("", "", " - ", {
          selectionAtEnd: !0,
          filterSelection: (H) => " - " + H.replace(/\n$/, "").replace(/\n/g, `
 - `) + `
`
        }) : T("", "", "", {
          filterValue: (H, P) => {
            if (F >= 0) {
              let me = re.replace(/^ - /, "");
              B = V + me, P.pos -= re.length - me.length;
            }
            return B + de;
          },
          filterSelection: (H) => H.replace(/^ - /g, "").replace(/\n - /g, `
`)
        });
      } else
        T(`
 - `, `
`, "List Item", { offsetStart: -10, offsetEnd: 9 });
    }
    function S() {
      const C = Y(), _ = C.indexOf(`
`) === -1;
      C ? _ ? T(`
## `, `
`, "") : T("## ", "", "") : T(`
## `, `
`, "Heading", { offsetStart: -8, offsetEnd: 7 });
    }
    function le() {
      let { sel: C, selPos: _, beforeSel: B, afterSel: de, prevCRPos: F, beforeCR: V, afterCR: re } = R();
      !C.startsWith("//") && !re.startsWith("//") ? C ? T("", "", "//", {
        selectionAtEnd: !0,
        filterSelection: (fe) => "//" + fe.replace(/\n$/, "").replace(/\n/g, `
//`) + `
`
      }) : N({
        value: V + "//" + re + de,
        selectionStart: _ + 2
      }) : T("", "", "", {
        filterValue: (fe, H) => {
          if (F >= 0) {
            let P = re.replace(/^\/\//, "");
            B = V + P, H.pos -= re.length - P.length;
          }
          return B + de;
        },
        filterSelection: (fe) => fe.replace(/^\/\//g, "").replace(/\n\/\//g, `
`)
      });
    }
    const x = () => T(`/*
`, `*/
`, "");
    function W() {
      if (i.length === 0)
        return !1;
      const C = g.value, _ = i.pop();
      return r.push({ value: C.value, selectionStart: C.selectionStart, selectionEnd: C.selectionEnd }), N(_), !0;
    }
    function E() {
      if (r.length === 0)
        return !1;
      const C = g.value, _ = r.pop();
      return i.push({ value: C.value, selectionStart: C.selectionStart, selectionEnd: C.selectionEnd }), N(_), !0;
    }
    const G = () => null;
    return Xe(() => {
      i = [], r = [];
      const C = g.value;
      C.onkeydown = (_) => {
        if (_.key === "Escape" || _.keyCode === 27) {
          l("close");
          return;
        }
        const B = String.fromCharCode(_.keyCode).toLowerCase();
        B === "	" ? (!_.shiftKey ? T("", "", "    ", {
          selectionAtEnd: !0,
          filterSelection: (F) => "    " + F.replace(/\n$/, "").replace(/\n/g, `
    `) + `
`
        }) : T("", "", "", {
          filterValue: (F, V) => {
            let { selPos: re, beforeSel: ye, afterSel: fe, prevCRPos: H, beforeCR: P, afterCR: me } = R();
            if (H >= 0) {
              let Ce = me.replace(/\t/g, "    ").replace(/^ ? ? ? ?/, "");
              ye = P + Ce, V.pos -= me.length - Ce.length;
            }
            return ye + fe;
          },
          filterSelection: (F) => F.replace(/\t/g, "    ").replace(/^ ? ? ? ?/g, "").replace(/\n    /g, `
`)
        }), _.preventDefault()) : _.ctrlKey ? B === "z" ? _.shiftKey ? E() && _.preventDefault() : W() && _.preventDefault() : B === "b" && !_.shiftKey ? (J(), _.preventDefault()) : B === "h" && !_.shiftKey ? (S(), _.preventDefault()) : B === "i" && !_.shiftKey ? (j(), _.preventDefault()) : B === "q" && !_.shiftKey ? (D(), _.preventDefault()) : B === "k" ? _.shiftKey ? (K(), _.preventDefault()) : (q(), _.preventDefault()) : B === "," || _.key === "<" || _.key === ">" || _.keyCode === 188 ? (ne(_), _.preventDefault()) : B === "/" || _.key === "/" ? (le(), _.preventDefault()) : (B === "?" || _.key === "?") && _.shiftKey && (x(), _.preventDefault()) : _.altKey && (_.key === "1" || _.key === "0" ? (ee(), _.preventDefault()) : _.key === "-" ? (te(), _.preventDefault()) : _.key === "s" && (L(), _.preventDefault()));
      };
    }), (C, _) => {
      var B;
      return a(), u("div", null, [
        Z(C.$slots, "header", Le({
          inputElement: g.value,
          id: e.id,
          modelValue: e.modelValue,
          status: e.status
        }, C.$attrs)),
        o(m) ? (a(), u("label", {
          key: 0,
          for: e.id,
          class: w(`mb-1 block text-sm font-medium text-gray-700 dark:text-gray-300 ${e.labelClass ?? ""}`)
        }, A(o(m)), 11, av)) : k("", !0),
        e.disabled ? k("", !0) : (a(), u("div", iv, [
          s("div", rv, [
            b("bold") ? (a(), u("svg", {
              key: 0,
              class: w(v),
              onClick: J,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, cv)) : k("", !0),
            b("italics") ? (a(), u("svg", {
              key: 1,
              class: w(v),
              onClick: j,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, vv)) : k("", !0),
            b("link") ? (a(), u("svg", {
              key: 2,
              class: w(v),
              onClick: q,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, pv)) : k("", !0),
            b("blockquote") ? (a(), u("svg", {
              key: 3,
              class: w(v),
              onClick: D,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, wv)) : k("", !0),
            b("image") ? (a(), u("svg", {
              key: 4,
              class: w(v),
              onClick: K,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, $v)) : k("", !0),
            b("code") ? (a(), u("svg", {
              key: 5,
              class: w(v),
              onClick: ne,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Lv)) : k("", !0),
            b("heading") ? (a(), u("svg", {
              key: 6,
              class: w(v),
              onClick: S,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Mv)) : k("", !0),
            b("orderedList") ? (a(), u("svg", {
              key: 7,
              class: w(v),
              icon: "",
              onClick: ee,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Fv)) : k("", !0),
            b("unorderedList") ? (a(), u("svg", {
              key: 8,
              class: w(v),
              onClick: te,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Ov)) : k("", !0),
            b("strikethrough") ? (a(), u("svg", {
              key: 9,
              class: w(v),
              onClick: L,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Bv)) : k("", !0),
            b("undo") ? (a(), u("svg", {
              key: 10,
              class: w(v),
              onClick: W,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Ev)) : k("", !0),
            b("redo") ? (a(), u("svg", {
              key: 11,
              class: w(v),
              onClick: E,
              xmlns: "http://www.w3.org/2000/svg",
              width: "24",
              height: "24",
              viewBox: "0 0 24 24"
            }, Uv)) : k("", !0),
            Z(C.$slots, "toolbarbuttons", {
              instance: (B = Be()) == null ? void 0 : B.exposed
            })
          ]),
          b("help") && e.helpUrl ? (a(), u("div", qv, [
            s("a", {
              title: "formatting help",
              target: "_blank",
              href: e.helpUrl,
              tabindex: "-1"
            }, [
              (a(), u("svg", {
                class: w(v),
                xmlns: "http://www.w3.org/2000/svg",
                width: "24",
                height: "24",
                viewBox: "0 0 24 24"
              }, Zv))
            ], 8, Qv)
          ])) : k("", !0)
        ])),
        s("div", Gv, [
          s("textarea", {
            ref_key: "txt",
            ref: g,
            name: e.id,
            id: e.id,
            class: w(o(p)),
            label: e.label,
            value: e.modelValue,
            rows: e.rows || 6,
            disabled: e.disabled,
            onInput: _[0] || (_[0] = (de) => {
              var F;
              return O(((F = de.target) == null ? void 0 : F.value) || "");
            }),
            onKeydown: Qn(G, ["tab"])
          }, null, 42, Wv)
        ]),
        o(c) ? (a(), u("p", {
          key: 2,
          class: "mt-2 text-sm text-red-500",
          id: `${e.id}-error`
        }, A(o(c)), 9, Jv)) : e.help ? (a(), u("p", {
          key: 3,
          class: "mt-2 text-sm text-gray-500",
          id: `${e.id}-description`
        }, A(e.help), 9, Xv)) : k("", !0),
        Z(C.$slots, "footer", Le({
          inputElement: g.value,
          id: e.id,
          modelValue: e.modelValue,
          status: e.status
        }, C.$attrs))
      ]);
    };
  }
}), eh = {
  key: 0,
  class: "relative z-10 lg:hidden",
  role: "dialog",
  "aria-modal": "true"
}, th = { class: "fixed inset-0 flex" }, lh = /* @__PURE__ */ s("span", { class: "sr-only" }, "Close sidebar", -1), nh = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6 text-white",
  fill: "none",
  viewBox: "0 0 24 24",
  "stroke-width": "1.5",
  stroke: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-linejoin": "round",
    d: "M6 18L18 6M6 6l12 12"
  })
], -1), sh = [
  lh,
  nh
], oh = { class: "flex grow flex-col gap-y-5 overflow-y-auto bg-white px-6 pb-2" }, ah = { class: "hidden lg:fixed lg:inset-y-0 lg:z-10 lg:flex lg:w-72 lg:flex-col" }, ih = { class: "flex grow flex-col gap-y-5 overflow-y-auto border-r border-gray-200 bg-white px-6" }, rh = {
  class: /* @__PURE__ */ w(["sticky top-0 flex items-center gap-x-6 bg-white px-4 py-4 shadow-sm sm:px-6 lg:hidden"])
}, uh = /* @__PURE__ */ s("span", { class: "sr-only" }, "Open sidebar", -1), dh = /* @__PURE__ */ s("svg", {
  class: "h-6 w-6",
  fill: "none",
  viewBox: "0 0 24 24",
  "stroke-width": "1.5",
  stroke: "currentColor",
  "aria-hidden": "true"
}, [
  /* @__PURE__ */ s("path", {
    "stroke-linecap": "round",
    "stroke-linejoin": "round",
    d: "M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5"
  })
], -1), ch = [
  uh,
  dh
], fh = /* @__PURE__ */ ue({
  __name: "SidebarLayout",
  setup(e, { expose: t }) {
    const { transition: l } = ns(), n = M(!0), i = M(""), r = {
      entering: { cls: "transition-opacity ease-linear duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "transition-opacity ease-linear duration-300", from: "opacity-100", to: "opacity-0" }
    }, d = M(""), c = {
      entering: { cls: "transition ease-in-out duration-300 transform", from: "-translate-x-full", to: "translate-x-0" },
      leaving: { cls: "transition ease-in-out duration-300 transform", from: "translate-x-0", to: "-translate-x-full" }
    }, m = M(""), h = {
      entering: { cls: "ease-in-out duration-300", from: "opacity-0", to: "opacity-100" },
      leaving: { cls: "ease-in-out duration-300", from: "opacity-100", to: "opacity-0" }
    };
    function y(v) {
      l(r, i, v), l(c, d, v), l(h, m, v), setTimeout(() => n.value = v, 300);
    }
    function b() {
      y(!0);
    }
    function p() {
      y(!1);
    }
    return t({ show: b, hide: p, toggle: y }), (v, g) => (a(), u("div", null, [
      n.value ? (a(), u("div", eh, [
        s("div", {
          class: w(["fixed inset-0 bg-gray-900/80", i.value])
        }, null, 2),
        s("div", th, [
          s("div", {
            class: w(["relative mr-16 flex w-full max-w-xs flex-1", d.value])
          }, [
            s("div", {
              class: w(["absolute left-full top-0 flex w-16 justify-center pt-5", m.value])
            }, [
              s("button", {
                type: "button",
                onClick: p,
                class: "-m-2.5 p-2.5"
              }, sh)
            ], 2),
            s("div", oh, [
              Z(v.$slots, "default")
            ])
          ], 2)
        ])
      ])) : k("", !0),
      s("div", ah, [
        s("div", ih, [
          Z(v.$slots, "default")
        ])
      ]),
      s("div", rh, [
        s("button", {
          type: "button",
          onClick: b,
          class: "-m-2.5 p-2.5 text-gray-700 lg:hidden"
        }, ch),
        Z(v.$slots, "mobiletitlebar")
      ])
    ]));
  }
}), mh = {
  Alert: jo,
  AlertSuccess: Zo,
  ErrorSummary: Yo,
  InputDescription: ta,
  Icon: Rs,
  Loading: Qa,
  OutlineButton: Ga,
  PrimaryButton: Xa,
  SecondaryButton: ti,
  TextLink: ni,
  Breadcrumbs: ui,
  Breadcrumb: vi,
  NavList: pi,
  NavListItem: Si,
  AutoQueryGrid: Ju,
  SettingsIcons: cd,
  FilterViews: Cn,
  FilterColumn: $n,
  QueryPrefs: _n,
  EnsureAccess: Es,
  EnsureAccessDialog: fd,
  TextInput: kd,
  TextareaInput: Md,
  SelectInput: Od,
  CheckboxInput: Nd,
  TagInput: fc,
  FileInput: Oc,
  Autocomplete: Jc,
  Combobox: e0,
  DynamicInput: t0,
  LookupInput: p0,
  AutoFormFields: y0,
  AutoForm: E0,
  AutoCreateForm: cf,
  AutoEditForm: Df,
  ConfirmDelete: Pf,
  FormLoading: Nf,
  DataGrid: Zf,
  CellFormat: Gf,
  PreviewFormat: t1,
  HtmlFormat: a1,
  CloseButton: c1,
  SlideOver: _1,
  ModalDialog: F1,
  ModalLookup: _m,
  Tabs: Dm,
  DarkModeToggle: Rm,
  SignIn: ov,
  MarkdownInput: Yv,
  SidebarLayout: fh
}, Rl = mh, wh = {
  install(e) {
    Object.keys(Rl).forEach((l) => {
      e.component(l, Rl[l]);
    });
    function t(l) {
      const i = Object.keys(l).filter((r) => l[r]).map((r) => `${encodeURIComponent(r)}=${encodeURIComponent(l[r])}`).join("&");
      return i ? "?" + i : "./";
    }
    e.directive("href", function(l, n) {
      l.href = t(n.value), l.onclick = (i) => {
        i.preventDefault(), history.pushState(n.value, "", t(n.value));
      };
    });
  },
  component(e, t) {
    return e ? t ? ie.components[e] = t : ie.components[e] || Rl[e] || null : null;
  }
};
export {
  bh as css,
  wh as default,
  kn as useAuth,
  Tl as useClient,
  St as useConfig,
  ph as useFiles,
  yh as useFormatters,
  ot as useMetadata,
  ns as useUtils
};
