/* jshint esversion: 6 */
/*
    Copyright 2016 0xFireball. All Rights Reserved.

    https://github.com/0xFireball
    https://0xfireball.me

    This file is part of NoteBin3
*/

function showMessage(text, duration) {
    duration = duration || 2400;
    Materialize.toast(text, duration);
}