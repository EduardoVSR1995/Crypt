using System;
using System.Threading.Tasks;
using Crypt;
public class Program : Operations
{
    public async static Task Main(string[] args)
    {
        ConfigureApplicationPath();

        if (args.Length < 4)
        {
            Console.WriteLine("Usage: Crypt [<enc>  || <dec>] [<srcFolder>  <destFile>] <password>");
            return;
        }

        //string srcFolder = "C:\\Users\\eduar\\Documents\\projetos_base\\bserpNotas";
        //string destFile = "C:\\Users\\eduar\\Documents\\Meus_projetos\\Crypt\\teste\\enc.txt";
        //string srcFolder = "C:\\Users\\eduar\\Documents\\Meus_projetos\\Crypt\\teste\\dec";
        //string destFile = "C:\\Users\\eduar\\Documents\\Meus_projetos\\Crypt\\teste\\enc.txt";

        string srcFolder = args[1];
        string destFile = args[2];

        Password = args[3];
        //Password = "super" ;
        string response = "";
        var start = DateTime.Now;

        switch (args[0])
        {
            case "enc": 
                await EncryptDirectoryToTxt(srcFolder, destFile);
                response = "encriptação";
                break;
            case "dec":
                response = "decriptação";
                await DecryptTxtToDirectory(destFile, srcFolder);
                break;
            default:
                Console.WriteLine(" enc or dec ");
                break;
        }

        var end = DateTime.Now;

        Console.WriteLine($" Tempo de { response } : { (end - start) }");
    }
}