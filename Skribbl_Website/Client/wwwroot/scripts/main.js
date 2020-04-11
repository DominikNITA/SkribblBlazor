
objectReferences = {};

addNewReference = function (reference, id) {
    objectReferences[id] = reference;
};

changeShowBan = function (newState, playerName) {
    let dotNetObject = objectReferences[playerName];
    dotNetObject.invokeMethodAsync('ChangeShowBanPlayer', newState);
};

//https://www.codicode.com/art/how_to_draw_on_a_html5_canvas_with_a_mouse.aspx

var mousePressed = false;
var lastX, lastY;
var ctx;
var isDrawing = false;
var boardDotNetRefernce;

function initThis(reference) {
    boardDotNetRefernce = reference;
    console.log(boardDotNetRefernce);
    ctx = document.getElementById('myCanvas').getContext("2d");

    $('#myCanvas').mousedown(function (e) {
        mousePressed = true;
        draw(e.pageX - $(this).offset().left, e.pageY - $(this).offset().top, false);
    });

    $('#myCanvas').mousemove(function (e) {
        if (mousePressed) {
            draw(e.pageX - $(this).offset().left, e.pageY - $(this).offset().top, true);
        }
    });

    $('#myCanvas').mouseup(function (e) {
        mousePressed = false;
    });
    $('#myCanvas').mouseleave(function (e) {
        mousePressed = false;
    });
}
let i = 0;
let jump = 2;
function draw(x, y, isDown) {
    if (isDrawing) {
        if (isDown) {
            ctx.beginPath();
            ctx.strokeStyle = "green";
            ctx.lineWidth = 3;
            ctx.lineJoin = "round";
            ctx.moveTo(lastX, lastY);
            ctx.lineTo(x, y);
            ctx.closePath();
            ctx.stroke();
            i++;
            if (i >= jump) {
                boardDotNetRefernce.invokeMethodAsync('SendDraw', x, y, isDown, "green", 3);
                i = 0;
            }
        }
        else {
            boardDotNetRefernce.invokeMethodAsync('SendDraw', x, y, isDown, "green", 3);
        }
        lastX = x; lastY = y;
    }
}

function drawFromOutside(x, y, isDown, color, size) {
    if (isDown) {
        ctx.beginPath();
        ctx.strokeStyle = color;
        ctx.lineWidth = size;
        ctx.lineJoin = "round";
        ctx.moveTo(lastX, lastY);
        ctx.lineTo(x, y);
        ctx.closePath();
        ctx.stroke();
    }
    lastX = x; lastY = y;
}

function clearArea() {
    // Use the identity matrix while clearing the canvas
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
}

function setIsDrawing(newIsDrawing) {
    isDrawing = newIsDrawing;
}

function prepareBoard() {
    setIsDrawing(false);
    clearArea();
}
