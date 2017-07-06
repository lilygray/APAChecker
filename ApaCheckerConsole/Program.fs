// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//open FSharp.Data
//open Microsoft.Net.Http
open System.Net.Http

[<EntryPoint>]

let main argv =
    //let xml = 
        //Http.RequestString( 
            //"http://localhost:8080/processFulltextDocument", httpMethod = "POST",
            //body = FormValues [ "input", "kelly-nocover.pdf" ],
            //headers = [ 
            //    HttpRequestHeaders.Accept HttpContentTypes.Text;
            //    HttpRequestHeaders.ContentType "multipart/form-data"; 
            //    //
            //    //HttpRequestHeaders.AcceptEncoding "gzip, deflate, br";
            //    //HttpRequestHeaders.Connection "keep-alive";
            //    ]
            //)

            //https://stackoverflow.com/questions/36745769/send-files-using-httpwebrequest
 //https://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data/16925159#16925159

//-----------------------------------------------------------------------------------

// Perform the equivalent of posting a form with a filename and two files, in HTML:
// <form action="{url}" method="post" enctype="multipart/form-data">
//     <input type="text" name="filename" />
//     <input type="file" name="file1" />
//     <input type="file" name="file2" />
// </form>
   // System.IO.Stream Upload url filename fileStream fileBytes

    // Convert each of the three inputs into HttpContent objects
    let url = "http://localhost:8080/processFulltextDocument"
    let filename = "kelly-nocover.pdf"


    //let fileStream = new System.IO.FileStream( filename, System.IO.FileMode.Open )
    let fileBytes = System.IO.File.ReadAllBytes( filename )

    let stringContent = new StringContent(filename);
    // examples of converting both Stream and byte [] to HttpContent objects
    // representing input type file
    //let fileStreamContent = new StreamContent(fileStream);
    let bytesContent = new ByteArrayContent(fileBytes);

    // Submit the form using HttpClient and 
    // create form data as Multipart (enctype="multipart/form-data")

    let client = new HttpClient()
    let formData = new MultipartFormDataContent()

    // Add the HttpContent objects to the form data

    // <input type="text" name="filename" />
    //formData.Add(stringContent, "filename", "filename");
    // <input type="file" name="file2" />
    formData.Add(bytesContent, "input");

    // Actually invoke the request to the server

    // equivalent to (action="{url}" method="post")
    let result = client.PostAsync(url, formData).Result
    let xml = result.Content.ReadAsStringAsync().Result;
   

    System.IO.File.WriteAllText("text.xml", xml)
    printfn "%A" argv
    0 // return an integer exit codeFSharp.Data