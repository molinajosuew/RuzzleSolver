using System;
using System.Collections.Generic;
using System.IO;

static class RuzzleSolver
{
    #region Classes

    class Tile
    {
        #region Enumerations

        public enum BonusType { NA, DL, TL, DW, TW }

        #endregion

        #region Properties

        public char Letter { get; set; }
        public BonusType Bonus { get; set; }
        public bool IsTaken { get; set; }

        #endregion

        #region Constructors

        public Tile(char letter, BonusType bonus)
        {
            Letter = letter;
            Bonus = bonus;
            IsTaken = false;
        }

        #endregion

        #region Methods

        public int Value()
        {
            switch (Letter)
            {
                case 'a':
                case 'e':
                case 'i':
                case 'l':
                case 'n':
                case 'o':
                case 'r':
                case 's':
                case 't':
                    return 1;
                case 'd':
                case 'u':
                    return 2;
                case 'g':
                case 'm':
                    return 3;
                case 'b':
                case 'c':
                case 'f':
                case 'h':
                case 'p':
                case 'v':
                case 'w':
                case 'y':
                    return 4;
                case 'k':
                    return 5;
                case 'x':
                    return 8;
                case 'j':
                case 'q':
                case 'z':
                    return 10;
                default:
                    return 0;
            }
        }

        #endregion
    }

    class TileWord
    {
        #region Properties

        public List<Tile> Tiles { get; set; }
        public string Word { get; set; }

        #endregion

        #region Constructors

        public TileWord(Tile tile)
        {
            Word += tile.Letter;
            Tiles = new List<Tile> { tile };
        }

        public TileWord(TileWord tileWord)
        {
            Word = tileWord.Word;
            Tiles = new List<Tile>();

            foreach (var tile in tileWord.Tiles)
            {
                Tiles.Add(tile);
            }
        }

        #endregion

        #region Methods

        public TileWord Add(Tile tile)
        {
            var result = new TileWord(this);
            result.Tiles.Add(tile);
            result.Word += tile.Letter;
            return result;
        }

        public int Value()
        {
            var result = 0;

            foreach (var tile in Tiles)
            {
                var multiplier = 1;

                switch (tile.Bonus)
                {
                    case Tile.BonusType.DL:
                        multiplier *= 2;
                        break;
                    case Tile.BonusType.TL:
                        multiplier *= 3;
                        break;
                }

                result += tile.Value() * multiplier;
            }

            foreach (var tile in Tiles)
            {
                switch (tile.Bonus)
                {
                    case Tile.BonusType.DW:
                        result *= 2;
                        break;
                    case Tile.BonusType.TW:
                        result *= 3;
                        break;
                }
            }

            if (Word.Length > 4)
            {
                result += (Word.Length - 4) * 5;
            }

            return result;
        }

        #endregion
    }

    #endregion

    #region Properties

    static HashSet<string> WordSet { get; set; }
    static HashSet<string> SubWordSet { get; set; }

    #endregion

    #region Constructors

    static RuzzleSolver()
    {
        var words = File.ReadAllLines(@"dictionary.txt");
        WordSet = new HashSet<string>(words);
        SubWordSet = new HashSet<string>();

        foreach (var word in words)
        {
            for (int i = 1; i <= word.Length; i++)
            {
                SubWordSet.Add(word.Substring(0, i));
            }
        }
    }

    #endregion

    #region Methods

    static void GetAllWords(Tile[] tiles, int index, TileWord candidate, List<TileWord> words)
    {
        if (!SubWordSet.Contains(candidate.Word))
        {
            return;
        }

        tiles[index].IsTaken = true;

        if (WordSet.Contains(candidate.Word))
        {
            words.Add(candidate);
        }

        for (int i = 0; i < 8; i++)
        {
            var row = index / 4 + (int)Math.Round(Math.Sin(Math.PI / 4 * (i - 2)));
            var col = index % 4 + (int)Math.Round(Math.Sin(Math.PI / 4 * i));

            if (row >= 0 && row < 4 && col >= 0 && col < 4 && !tiles[row * 4 + col].IsTaken)
            {
                GetAllWords(tiles, row * 4 + col, candidate.Add(tiles[row * 4 + col]), words);
            }
        }

        tiles[index].IsTaken = false;
    }

    #endregion

    static void Main()
    {
        while (true)
        {
            #region Input

            Console.Write("Letters: ");
            var letters = Console.ReadLine();

            Console.Write("Bonuses: ");
            var bonuses = Console.ReadLine();

            #endregion

            #region Initialize Data Structure

            var tiles = new Tile[16];

            for (int i = 0; i < letters.Length; i++)
            {
                tiles[i] = new Tile(letters[i], (Tile.BonusType)int.Parse(bonuses[i].ToString()));
            }

            #endregion

            #region Run Algorithm

            var words = new List<TileWord>();

            for (int i = 0; i < tiles.Length; i++)
            {
                GetAllWords(tiles, i, new TileWord(tiles[i]), words);
            }

            #endregion

            #region Output

            words.Sort(delegate(TileWord a, TileWord b)
            {
                return b.Value().CompareTo(a.Value());
            });

            Console.Write("Best Words: ");

            foreach (var word in words)
            {
                Console.Write(word.Word + " (" + word.Value() + "), ");
            }

            Console.WriteLine();
            Console.WriteLine();

            #endregion
        }
    }
}