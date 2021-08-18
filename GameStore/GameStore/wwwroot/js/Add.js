function Listen() {
    document.getElementById("first").addEventListener('click', () => {
        var firstDiv = document.getElementById("info");
        var secondDiv = document.getElementById("minReq");
        firstDiv.style.display = "none";
        secondDiv.style.display = "block";
    });

    document.getElementById("secondBack").addEventListener('click', () => {
        var firstDiv = document.getElementById("info");
        var secondDiv = document.getElementById("minReq");
        firstDiv.style.display = "block";
        secondDiv.style.display = "none";
    });

    document.getElementById("secondNext").addEventListener('click', () => {
        var secondDiv = document.getElementById("minReq");
        var thirdDiv = document.getElementById("recReq");
        secondDiv.style.display = "none";
        thirdDiv.style.display = "block";
    });

    document.getElementById("thirdBack").addEventListener('click', () => {
        var secondDiv = document.getElementById("minReq");
        var thirdDiv = document.getElementById("recReq");
        secondDiv.style.display = "block";
        thirdDiv.style.display = "none";
    });
}