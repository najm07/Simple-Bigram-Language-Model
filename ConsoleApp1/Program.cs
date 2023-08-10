using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SimpleLanguageModel
{
    class Program
    {
        static void Main()
        {
            // Define the value of N.
            int N = 2;

            // Initialize the dictionary.
            Dictionary<string, Dictionary<string, NWord>> ngrams = new();

            // Read the training corpus.
            string? line;
            using (StreamReader reader = new("training.txt"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    for (int i = 0; i <= words.Length - N; i++)
                    {
                        string ngram = string.Join(" ", words, i, N);
                        //string ngram = words[i];
                        if (!ngrams.TryGetValue(ngram, out Dictionary<string, NWord> ng))
                        {
                            ng = new Dictionary<string, NWord>();
                            ngrams.Add(ngram, ng);
                        }

                        if (i < words.Length - 2)
                        {
                            bool we = ng.TryGetValue(words[i + 2], out NWord nw);

                            nw ??= new NWord();

                            if (!we)
                                ng.Add(words[i + 2], nw);

                            nw.frequency++;
                        }
                    }
                }
            }

            // Calculate the probability of each n-gram.
            ngrams = new Dictionary<string, Dictionary<string, NWord>> (CalculateProbabilities(ngrams));
            

            // Prompt the user for a sentence.
            Console.WriteLine("Enter a sentence: ");
            line = Console.ReadLine();
            
            Console.WriteLine("Point #1");

            // Predict the next word in the sentence.
            string[] inputWords = line.Split(' ');

            string ngram1 = string.Join(" ", inputWords, inputWords.Length - 2, N);
            if (ngrams.TryGetValue(ngram1, out Dictionary<string, NWord> ng2))
            {
                Debug.WriteLine("Point #2");
                // Find the word with the highest probability.
                string nextWord = ng2.Keys.First();
                float maxProbability = ng2.Values.First().probability;

                foreach (var w in ng2)
                {
                    Debug.WriteLine("Point #3");
                    if (w.Value.probability > maxProbability)
                    {
                        maxProbability = w.Value.probability;
                        nextWord = w.Key;
                    }
                }

                // Print the next word.
                Console.Write(line + " " + nextWord);
                Debug.WriteLine("Point #4");
            }
            Debug.WriteLine("Point #5");

            Console.WriteLine();
        }

        private static IDictionary<string, Dictionary<string, NWord>> CalculateProbabilities(Dictionary<string, Dictionary<string, NWord>> ngrams)
        {
            foreach (Dictionary<string, NWord> ng in ngrams.Values)
            {
                foreach (NWord nw in ng.Values)
                {
                    if (nw.frequency > 0)
                    {
                        nw.probability = (float)nw.frequency / (float)ng.Count;
                    }
                }
            }

            return ngrams;
        }
    }

    class NWord
    {
        public int frequency;
        public float probability;

        public NWord()
        {
            this.frequency = 0;
            this.probability = 0.0f;
        }
    }
}
