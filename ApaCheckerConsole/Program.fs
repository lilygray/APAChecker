// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//open FSharp.Data
//open Microsoft.Net.Http
open System.Net.Http
open System.Xml.Serialization
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open Stanford.NLP.FSharp

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
    //let result = client.PostAsync(url, formData).Result
    //let xml = result.Content.ReadAsStringAsync().Result;

    //System.IO.File.WriteAllText("text.xml", xml)
    //printfn "%A" argv
   
    let xn s = XName.Get(s, "http://www.tei-c.org/ns/1.0")
    let doc = XDocument.Load("text.xml")

    //let abstractElement = doc.XPathSelectElement("/TEI")
    let abstractElement = doc.Element(xn "TEI").Element(xn "teiHeader").Element(xn "profileDesc").Element(xn "abstract").Element(xn "p").Value
    let titleElement = doc.Element(xn "TEI").Element(xn "teiHeader").Element(xn "fileDesc").Element(xn "titleStmt").Element(xn "title").Value
    let absWords = 
        abstractElement.Split([|' '|]) 
        |> Array.map (fun s -> s.Trim())

    let absWordLength = absWords.Length
    if absWords.Length > 250 then printfn "Most journals have a word limit between 150 and 250. Your abstract is too long at %i words." absWords.Length
    if (150 < absWords.Length && absWords.Length < 250) then printfn "Most journals have a word limit between 150 and 250. Your abstract may be too long at %i words. Check with your particular journal." absWords.Length
    if abstractElement.Contains(titleElement) then printfn "You have a limited number of words. Don't waste them by repeating your title!"

    //printfn "%s" "hi" //(par.Attribute(XName.Get "p").Value)

    for div in doc.Element(xn "TEI").Element(xn "text").Element(xn "body").Elements(xn "div") do
        for parg in div.Elements(xn "p") do
            printfn "%s" (parg.Value)

    for div in doc.Element(xn "TEI").Element(xn "text").Element(xn "body").Elements(xn "div") do
        for parg in div.Elements(xn "p") do
            for rf in parg.Elements(xn "ref") do
               // if rf.Attribute(xn "type").Value = "bibr" then //no errors? but it's throwing a NullReferenceException
                if (rf.Value.Contains("and") || rf.Value.Contains("&")) then printfn "%s" (rf.Value)

    0 // return an integer exit codeFSharp.Data
