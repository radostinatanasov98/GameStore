function Listen() {
    document.getElementById("gamesBtn").addEventListener('click', () => {
        var gamesDiv = document.getElementById("games");
        var reviewsDiv = document.getElementById("reviews");

        gamesDiv.style.display = "";
        reviewsDiv.style.display = "none";
    });

    document.getElementById("reviewsBtn").addEventListener('click', () => {
        var gamesDiv = document.getElementById("games");
        var reviewsDiv = document.getElementById("reviews");

        gamesDiv.style.display = "none";
        reviewsDiv.style.display = "block";
    })
}