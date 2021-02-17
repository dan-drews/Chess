window.PlaySound = function () {
    document.getElementById('sound').play();
    setTimeout(function () {
        document.getElementById('sound').stop()
        document.getElementById('sound').currentTime = 0;
    }, 1000);
}