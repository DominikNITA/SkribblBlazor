
objectReferences = {};

addNewReference = function(reference, id) {
    objectReferences[id] = reference;
};

changeShowBan = function(newState, playerName) {
    let dotNetObject = objectReferences[playerName];
    dotNetObject.invokeMethodAsync("ChangeShowBanPlayer", newState);
};

//https://www.codicode.com/art/how_to_draw_on_a_html5_canvas_with_a_mouse.aspx

var mousePressed = false;
var lastX, lastY;
var ctx;
var customCursor;
var isDrawing = false;
var lineWidth = 3;
var color = "black";
var boardWidth, boardHeight;
var wrapper;
var canvas;
var boardDotNetRefernce;

function initThis(reference) {
    boardDotNetRefernce = reference;
    console.log(boardDotNetRefernce);
    canvas = document.getElementById("myCanvas");
    ctx = document.getElementById("myCanvas").getContext("2d");
    customCursor = document.getElementById("customCursor");
    wrapper = document.getElementById("board-area");

    //Avoid blur
    ctx.webkitImageSmoothingEnabled = false;
    ctx.mozImageSmoothingEnabled = false;
    ctx.imageSmoothingEnabled = false;

    setLineWidth(2);
    setColor("black");

    $("#canvasOverlay").mousedown(function(e) {
        mousePressed = true;
        draw(e.pageX - $(this).offset().left, e.pageY - $(this).offset().top, false);
        draw(e.pageX - $(this).offset().left, e.pageY - $(this).offset().top, true);
    });

    $("#canvasOverlay").mousemove(function(e) {
        customCursor.style.top = (e.pageY - convertToActualLineWidth(lineWidth) / 2) + "px";
        customCursor.style.left = (e.pageX - convertToActualLineWidth(lineWidth) / 2) + "px";
        if (mousePressed) {
            draw(e.pageX - $(this).offset().left, e.pageY - $(this).offset().top, true);
        }
    });

    $("#canvasOverlay").mouseup(function(e) {
        mousePressed = false;
    });
    $("#canvasOverlay").mouseleave(function(e) {
        mousePressed = false;
        customCursor.style.display = "none";
    });
    $("#canvasOverlay").mouseenter(function(e) {
        customCursor.style.display = "block";
    });

    window.addEventListener("resize", fitBoardToWrapper);
    fitBoardToWrapper();
}

//TODO: Refactor below
function draw(x, y, isDown) {
    if (isDrawing) {
        if (isDown) {
            ctx.beginPath();
            if (x == lastX && y == lastY) {
                ctx.fillStyle = color;
                ctx.arc(x, y, convertToActualLineWidth(lineWidth) / 2, 0, Math.PI * 2, true);
                ctx.fill();
            } else {
                ctx.strokeStyle = color;
                ctx.lineWidth = convertToActualLineWidth(lineWidth);
                ctx.lineJoin = "round";
                ctx.moveTo(lastX, lastY);
                ctx.lineTo(x, y);
                ctx.closePath();
                ctx.stroke();
            }
            boardDotNetRefernce.invokeMethodAsync("SendDraw",
                x / boardWidth,
                y / boardHeight,
                isDown,
                color,
                lineWidth);
        } else {
            boardDotNetRefernce.invokeMethodAsync("SendDraw",
                x / boardWidth,
                y / boardHeight,
                isDown,
                color,
                lineWidth);
        }
        lastX = x;
        lastY = y;
    }
}

function drawFromOutside(xPercent, yPercent, isDown, color, size) {
    var x = xPercent * boardWidth;
    var y = yPercent * boardHeight;
    if (isDown) {
        ctx.beginPath();
        if (x == lastX && y == lastY) {
            ctx.fillStyle = color;
            ctx.arc(x, y, convertToActualLineWidth(size) / 2, 0, Math.PI * 2, true);
            ctx.fill();
        } else {
            ctx.strokeStyle = color;
            ctx.lineWidth = convertToActualLineWidth(size);
            ctx.lineJoin = "round";
            ctx.moveTo(lastX, lastY);
            ctx.lineTo(x, y);
            ctx.closePath();
            ctx.stroke();
        }
    }
    lastX = x;
    lastY = y;
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

function setColor(newColor) {
    color = newColor;
    customCursor.style.backgroundColor = newColor;
}

function setLineWidth(newLineWidth) {
    lineWidth = newLineWidth;
    customCursor.style.width = convertToActualLineWidth(newLineWidth) + "px";
    customCursor.style.height = convertToActualLineWidth(newLineWidth) + "px";
    customCursor.style.borderRadius = convertToActualLineWidth(newLineWidth) + "px";
}

function convertToActualLineWidth(relativeLineWidth) {
    return relativeLineWidth * boardHeight / 100;
}

function fitBoardToWrapper() {
    console.log("in fitBoardToWrapper");
    var tempCanvas = document.createElement("canvas");
    var tempContext = tempCanvas.getContext("2d");

    tempCanvas.width = boardWidth;
    tempCanvas.height = boardHeight;

    //avoid blur
    tempContext.webkitImageSmoothingEnabled = false;
    tempContext.mozImageSmoothingEnabled = false;
    tempContext.imageSmoothingEnabled = false;
    tempContext.drawImage(canvas, 0, 0);

    boardWidth = wrapper.offsetWidth;
    boardHeight = wrapper.offsetHeight;

    canvas.style.width = boardWidth + "px";
    canvas.style.height = boardHeight + "px";
    canvas.width = boardWidth;
    canvas.height = boardHeight;
    ctx.drawImage(tempCanvas, 0, 0, tempCanvas.width, tempCanvas.height, 0, 0, boardWidth, boardHeight);
}