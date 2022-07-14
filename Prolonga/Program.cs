using Antlr4.Runtime;
using Prolonga;

var filename = "prolog.txt";

var fileContent = File.ReadAllText(filename);

var inputStream = new AntlrInputStream(fileContent);
Console.WriteLine("String archivo: "+fileContent);
prologLexer Lexer = new prologLexer(inputStream);
var commonTokenStream = new CommonTokenStream(Lexer);
var prologParser = new prologParser(commonTokenStream);
var prologContext = prologParser.p_text();
var visitor = new Verificador();
visitor.Visit(prologContext);
visitor.printClausulas();

