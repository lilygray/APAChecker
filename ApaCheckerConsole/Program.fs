// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

//open FSharp.Data
//open Microsoft.Net.Http
open System.Net.Http
open System.Xml.Serialization
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
//open Stanford.NLP.FSharp
//open HtmlAgilityPack
open System.Text.RegularExpressions

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

    //let xn s = XName.Get(s, "http://www.tei-c.org/ns/1.0")
    //let doc = XDocument.Load("text.xml")

    let doc = new XmlDocument()

    //dial the area code
    let nsmgr = new XmlNamespaceManager(doc.NameTable)
    nsmgr.AddNamespace("TEI", "http://www.tei-c.org/ns/1.0")

    doc.Load("text.xml")
    let xmlNode = doc.DocumentElement :> XmlNode

    //function to get all one element with a specific name. This way we don't have to manually dial the "area code" every time.
    let getElement elementName =
        xmlNode.SelectSingleNode("//TEI:" + elementName, nsmgr).InnerText

    //function to get all the elements with a specific name, using mapping. This way we don't have to manually dial the "area code" every time.
    let getElements elementName =
        seq{
            let nodes = xmlNode.SelectNodes("//TEI:" + elementName, nsmgr)
            for node in nodes do
                yield node
            }
         |> Seq.map(fun n -> n.InnerText)
    
    //function that will use regex to identify numbers in English
    let regexEnglishNumbers input =
        let numberRegex = new Regex(@"\b(zero|four|eight|(?:fiv|(?:ni|o)n)e|t(?:wo|hree)|s(?:ix|even)|twelve|(?:(?:elev|t)e|(?:fif|eigh|nine|(?:thi|fou)r|s(?:ix|even))tee)n|(?:fif|six|eigh|nine|(?:tw|sev)en|(?:thi|fo)r)ty|hundred|thousand|million|billion|trillion)\b")
        //(from http://www.rexegg.com/regex-trick-numbers-in-english.html)
        //return a list of numbers
        //MAKE A LIST TO APPEND WORDS TO, SPLIT INPUT INTO WORDS, ADD WORDS TO LIST IF THEY MATCH
        seq numWords
        for word in n
        if numberRegex.IsMatch(word) then
            numberRegex.append(word)

    //function that will get and run tests on the abstract
    let abstractTests =
        let abstractElement = getElement "abstract"

        //split the abstract chunk into separate words
        let absWords = 
            abstractElement.Split([|' '|]) 
            |> Array.map (fun s -> s.Trim())

        //test to see if the abstract is the right length
        let absWordLength = absWords.Length
        if absWords.Length > 250 then printfn "Most journals have a word limit between 150 and 250. Your abstract is too long at %i words.\n" absWords.Length
        if (150 < absWords.Length && absWords.Length < 250) then printfn "Most journals have a word limit between 150 and 250. Your abstract may be too long at %i words. Check with your particular journal.\n" absWords.Length

        let titleElement = getElement "title"
        if abstractElement.Contains(titleElement) then printfn "You have a limited number of words. Don't waste them by repeating your title!\n"

        //if abstract contains a number it should be a numeral not a word
        let absNumList = regexEnglishNumbers absWords
        for word in absNumList do
            printfn "Numbers in abstracts should be expressed as numerals. Please check the following word: %s\n" word

    let paragraphs = getElements "p"

    //function to get and run tests on the references
    let refTests =
        let references = getElements "ref"
        let refsWithAnd = references |> Seq.filter(fun ref -> ref.Contains("and"))

        for ref in refsWithAnd do
            printfn "References should contain '&' instead of the word 'and.' Please check the following reference: %s\n" ref

        //if rf.Attribute(xn "type").Value = "bibr" then //no errors? but it's throwing a NullReferenceException
        //    if (rf.Value.Contains("and") || rf.Value.Contains("&")) then printfn "%s" (rf.Value)

    //biblstruct children "title"

    abstractTests
    refTests

    0 // return an integer exit code