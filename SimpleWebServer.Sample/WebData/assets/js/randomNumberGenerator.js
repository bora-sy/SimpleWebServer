function generateRandom()
{
    // This would be much easier to do with JS only but the point is doing requests to the server

    const url = "/api/generaterandomnumber";
    
        fetch(url)
            .then(response => response.text())
            .then(data => {
                
                document.getElementById('generatednum').innerHTML = "Generated Number: " + data;
            })
            .catch(error => {
                console.error('Error:', error);
                alert("An error occured: " + error);
            });
}