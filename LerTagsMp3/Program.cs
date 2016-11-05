using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace LerTagsMp3
{
    class Program
    {
        private static string PASTA_RAIZ = "C:\\simple-player-front\\musicas";
        private static IMongoClient _client;
        private static IMongoDatabase _database;
        static void Main(string[] args)
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("simpleplayer");
            var collection = _database.GetCollection<BsonDocument>("musicas");

            int faixasImportadas = 0;
            int faixasNaoImportadas = 0;
            foreach (string musica in Directory.EnumerateFiles(PASTA_RAIZ, "*.mp3", SearchOption.AllDirectories))
            {
                try
                {
                    TagLib.File file = TagLib.File.Create(musica);
                    string url = musica.Replace(PASTA_RAIZ, "").Replace("\\", "/");
                    string titulo = file.Tag.Title ?? "";
                    string artista = file.Tag.FirstPerformer ?? "";
                    string album = file.Tag.Album ?? "";
                    string genero = file.Tag.JoinedGenres ?? "";
                    string duracao = file.Properties.Duration.ToString().Split('.')[0];
                    string ano = file.Tag.Year != 0 ? file.Tag.Year.ToString() : "";
                    string faixa = file.Tag.Track != 0 ? file.Tag.Track.ToString() : "";

                    BsonDocument doc = new BsonDocument() {
                        { "url", url },
                        { "titulo",  titulo},
                        { "artista",  artista},
                        { "album",  album},
                        { "genero",  genero},
                        { "duracao",  duracao},
                        { "ano",  ano},
                        { "faixa",  faixa},
                    };

                  
                        collection.InsertOne(doc);
                    
                    faixasImportadas++;
                }
                catch (CorruptFileException)
                {
                    faixasNaoImportadas++;
                    Console.Write("Deu pau nessa: " + musica);
                }
            }
            Console.WriteLine($"faixas importadas : {faixasImportadas.ToString()}");
            Console.WriteLine($"faixas não importadas : {faixasNaoImportadas.ToString()}");
            Console.ReadKey();
        }
    }
}
