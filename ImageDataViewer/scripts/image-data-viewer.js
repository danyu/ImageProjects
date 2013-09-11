function toBinary(str) {
    for (var i = 0; i < str.length; i++) {

  }

}


function hexToBase64(str) {
    return btoa(String.fromCharCode.apply(null, str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" ")));
}

function utf8_to_b64(str) {
    return window.btoa(unescape(encodeURIComponent(str)));
}

function b64_to_utf8(str) {
    return decodeURIComponent(escape(window.atob(str)));
}

// Usage:
//utf8_to_b64('✓ à la mode'); // "4pyTIMOgIGxhIG1vZGU="
//b64_to_utf8('4pyTIMOgIGxhIG1vZGU='); // "✓ à la mode"

