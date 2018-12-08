using System;

class Words
{
    string word;
    string translation;

    public string Word
    {
        set { word = value; }
        get { return word; }
    }
    public string Transllation
    {
        set { translation = value; }
        get { return translation; }
    }

    public Words(string w, string t)
    {
        word = w;
        translation = t;
    }
    public string Info()
    {
        return string.Format("Word {0}, Translation: {1}", word, translation);
    }
}

class Translator
{
    Words[] words;
    int index = 0;

    public string this[int index]
    {
        get
        {
            if (index >= 0 && index < words.Length)
            {
               return words[index].Info();
            }
            else
            {
                return string.Format("No such index");
            }
        }
    }

    public int this[string word]
    {
        get
        {
            for (int i=0; i<words.Length; i++)
            {
                if (Convert.ToString(words[i].Word)==word)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public Translator(int x)
    {
        words = new Words[x];
    }

    public void AddWord(Words word)
    {
        if (index != words.Length)
        {
            words[index] = word;
            index++;
        }
        else
        {
            Console.WriteLine("Translator is full!");
        }
    }
    
}
namespace Dictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Translator tr = new Translator(4);
            Words word1 = new Words("cat", "kot");
            Words word2 = new Words("dog","sobaka");
            Words word3 = new Words("parrot","papugaj");
            Words word4 = new Words("cow","Korowa");

            tr.AddWord(word1);
            tr.AddWord(word2);
            tr.AddWord(word3);
            tr.AddWord(word4);

            Console.WriteLine(tr[1]);
            Console.WriteLine(tr["cat"]);
            Console.WriteLine(tr["dog"]);
            Console.WriteLine(tr["dssog"]);
            Console.WriteLine(tr[11]);
            Console.ReadKey();
        }
    }
}
