using Antlr4.Runtime;
using Prolonga;
using System.Text.RegularExpressions;

string ruta = getRuta();
BaseDeConocimiento basedeconocimiento = getConocimiento(ruta);
MotorDeInferencia motorDeInferencia = new MotorDeInferencia(basedeconocimiento);
bool continuar = true;
while (continuar)
{
    Console.WriteLine("Escoja una opcion: " +
        "\n1. Consultar" +
        "\n2. Salir");
    string opcion = Console.ReadLine();
    if (opcion == "1")
    {
        Console.WriteLine("\nIngrese la consulta:");
        Consulta? consulta = getConsulta();
        if(consulta is not null)
        {
            Console.WriteLine("Se imprimiran los detalles de consulta");
            Console.WriteLine(consulta.ToString());
            
            getRespuesta(consulta,basedeconocimiento,motorDeInferencia);
        }
    }
    else if (opcion == "2")
    {
        continuar = false;
    }
    else
    {
        Console.WriteLine("\nOpcion invalida");
    }
}

void getRespuesta(Consulta consulta,BaseDeConocimiento baseDeConocimiento, MotorDeInferencia motorDeInferencia)
{
    Clausula? tipoRespuesta = baseDeConocimiento.consultar(consulta);
    if(tipoRespuesta is null)
    {
        consulta.respuesta = "No se encontró el procedimiento '"+consulta.predicado+"'";
    }else if(tipoRespuesta is Hecho)
    {
        consulta
    }else if (tipoRespuesta is Propiedad) { 
        
    }
    //buscar tipo de conocimiento que pueda contener la respuesta en la base de conocimientos.
    //Si es un Hecho se extrae los datos y se rellenan los datos de consulta.
    //Si es una regla escoger tipo de encadenamiento (1 hacia atras, 2 hacia adelante, 3 mixto).
    //Antes de aplicar encadenamiento hay que preguntar si se quiere una traza.
    //Si no se encontró el predicado en ninguno de las bases entonces hay que indicar que no existe el procedimiento.
    //Dependiendo del tipo de argumento que tenga la consulta la respuesta
}

Consulta getConsulta()
{
    string consultaString = Console.ReadLine();
    if (isConsultaValida(consultaString))
    {
        return getValoresConsulta(consultaString+"\0");
    }
    return null;
}

Consulta getValoresConsulta(string consulta)
{
    AntlrInputStream inputStream = new AntlrInputStream(consulta);
    prologLexer Lexer = new prologLexer(inputStream);
    CommonTokenStream commonTokenStream = new CommonTokenStream(Lexer);
    prologParser prologParser = new prologParser(commonTokenStream);
    var prologContext = prologParser.p_text();
    ExtractorConsulta verificador = new ExtractorConsulta();

    verificador.Visit(prologContext);
    return verificador.consulta;
}

bool isConsultaValida(string? consultaString)
{
    Regex rxConsulta = new Regex(@"\?\-(\s)+[a-z]([a-z]|[0-9]|[A-Z])+\((\s)*([A-Z]|_|[a-z]([A-Z]|[a-z]|[0-9])*)(\s)*(\,(\s)*([A-Z]|_|[a-z]([A-Z]|[a-z]|[0-9])*)(\s)*)*\)(\s)*.");
    if (rxConsulta.IsMatch(consultaString))
    {
        Console.WriteLine("\nConsulta valida");
        return true;
    }
    Console.WriteLine("\nConsulta invalida");
    return false;
}

BaseDeConocimiento getConocimiento(string ruta)
{
    string fileContent = File.ReadAllText(ruta);
    AntlrInputStream inputStream = new AntlrInputStream(fileContent);
    prologLexer Lexer = new prologLexer(inputStream);
    CommonTokenStream commonTokenStream = new CommonTokenStream(Lexer);
    prologParser prologParser = new prologParser(commonTokenStream);
    var prologContext = prologParser.p_text();
    ExtractorArchivo verificador = new ExtractorArchivo();
    
    verificador.Visit(prologContext);
    verificador.printClausulas();
    
    BaseDeHechos baseDeHechos = new BaseDeHechos(verificador.clausulas);
    baseDeHechos.printHechos();
    
    BaseDeReglas baseDeReglas = new BaseDeReglas(verificador.clausulas);
    baseDeReglas.printReglas();
    
    return new BaseDeConocimiento(baseDeHechos,baseDeReglas);
}

string getRuta(){
    string ruta = "";
    do
    {
        Console.WriteLine("\nPara comenzar ingrese la ruta del archivo de conocimiento .pl");
        ruta = Console.ReadLine();
    } while (!isRutaValida(ruta));
    return ruta;
}

bool isRutaValida(string ruta)
{
    bool rutaValida = true;
    if (!File.Exists(ruta))
    {
        rutaValida = false;
        Console.WriteLine("\nEl archivo no existe");
    }
    if (!ruta.EndsWith(".pl"))
    {
        rutaValida = false;
        Console.WriteLine("\nEl archivo no es de extensión .pl");
    }
    return rutaValida;
}