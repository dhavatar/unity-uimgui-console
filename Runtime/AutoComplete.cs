using System.Collections.Generic;

namespace UImGuiConsole
{
    /// <summary>
    /// Autocomplete ternary search tree.
    /// </summary>
    public class AutoComplete
    {
        private class Node
        {
            public Node(char ch, bool isWord = false)
            {
                Value = ch;
                IsWord = isWord;
            }

            public char Value { get; set; }
            
            public bool IsWord { get; set; }

            public Node Less = null;
            public Node Equal = null;
            public Node Greater = null;
        }

        /// <summary>
        /// Number of tree nodes.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Number of words in the tree.
        /// </summary>
        public int Count { get; private set; }

        private Node node;

        public AutoComplete() { }

        public AutoComplete(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Insert(item);
            }
        }

        /// <summary>
        /// Search if the given word is in the tree.
        /// </summary>
        /// <param name="word">Word to search.</param>
        /// <returns>True if the word was found.</returns>
        public bool Search(string word)
        {
            Node ptr = node;
            int index = 0;

            // Traverse tree in look for the given string.
            while (ptr != null)
            {
                if (word[index] < ptr.Value)
                {
                    ptr = ptr.Less;
                }
                else if (word[index] == ptr.Value)
                {
                    // Word was found
                    if (index + 1 == word.Length && ptr.IsWord)
                    {
                        return true;
                    }

                    ptr = ptr.Equal;
                    ++index;
                }
                else
                {
                    ptr = ptr.Greater;
                }
            }

            return false;
        }

        /// <summary>
        /// Insert the word into the tree.
        /// </summary>
        /// <param name="word">Word to insert.</param>
        public void Insert(string word)
        {
            ref Node ptr = ref node;
            int index = 0;
            ++Count;

            while (index < word.Length)
            {
                if (ptr == null)
                {
                    ptr = new Node(word[index]);
                    ++Size;
                }

                if (word[index] < ptr.Value)
                {
                    ptr = ref ptr.Less;
                }
                else if (word[index] == ptr.Value)
                {
                    // String is already in tree, therefore only mark as word.
                    if (index + 1 == word.Length)
                    {
                        if (ptr.IsWord)
                        {
                            --Count;
                        }
                        ptr.IsWord = true;
                    }

                    ptr = ref ptr.Equal;
                    ++index;
                }
                else
                {
                    ptr = ref ptr.Greater;
                }
            }
        }

        /// <summary>
        /// Removes a word from the tree if found.
        /// </summary>
        /// <param name="word">String to be removed.</param>
        public void Remove(string word)
        {
            RemoveAux(node, word);   
        }

        /// <summary>
        /// Retrieve suggestions that match the given prefix.
        /// </summary>
        /// <param name="prefix">Prefix string to find in the tree.</param>
        /// <param name="options">Output list of string options matching the prefix.</param>
        public void Suggestions(string prefix, List<string> options)
        {
            ref Node ptr = ref node;
            int index = 0;

            while (ptr != null)
            {
                if (prefix[index] < ptr.Value)
                {
                    ptr = ref ptr.Less;
                }
                else if (prefix[index] == ptr.Value)
                {
                    // Prefix exists in tree.
                    if (index + 1 == prefix.Length)
                    {
                        break;
                    }

                    ptr = ref ptr.Equal;
                    ++index;
                }
                else
                {
                    ptr = ref ptr.Greater;
                }
            }

            // Already a word. (No need to auto complete).
            if (ptr != null && ptr.IsWord) return;

            // Prefix is not in tree.
            if (ptr == null) return;

            // Retrieve auto complete options.
            SuggestionsAux(ptr.Equal, options, prefix);
        }

        /// <summary>
        /// Store suggestions that match the previx and return partially completed prefix if possible.
        /// </summary>
        /// <param name="prefix">Prefix string to find in the tree.</param>
        /// <param name="options">Output list of string options matching the prefix.</param>
        /// <returns>Partially completed prefix.</returns>
        public string PartialSuggestions(string prefix, List<string> options)
        {
            string temp = prefix.ToString();
            Suggestions(ref temp, options, true);
            return temp;
        }

        /// <summary>
        /// Retrieve suggestions that match the given prefix.
        /// </summary>
        /// <param name="prefix">Prefix string to find in the tree, will be partially completed if flag partialComplete is on.</param>
        /// <param name="options">Output list of string options matching the prefix.</param>
        /// <param name="partialComplete">Flag to determine if the prefix string will be partially completed.</param>
        public void Suggestions(ref string prefix, List<string> options, bool partialComplete)
        {
            Node ptr = node;
            string temp = prefix;
            int index = 0;

            // Traverse tree and check if prefix exists.
            while (ptr != null)
            {
                if (prefix[index] < ptr.Value)
                {
                    ptr = ptr.Less;
                }
                else if (prefix[index] == ptr.Value)
                {
                    // Prefix exists in tree.
                    if (index + 1 == prefix.Length)
                    {
                        if (partialComplete)
                        {
                            Node pcPtr = ptr.Equal;

                            // Get partially completed string.
                            while (pcPtr != null)
                            {
                                if (pcPtr.Equal != null && pcPtr.Less == null && pcPtr.Greater == null)
                                {
                                    prefix += pcPtr.Value;
                                }
                                else
                                {
                                    break;
                                }

                                pcPtr = pcPtr.Equal;
                            }
                        }
                        break;
                    }

                    ptr = ptr.Equal;
                    ++index;
                }
                else
                {
                    ptr = ptr.Greater;
                }
            }

            // Already a word. (No need to auto complete).
            if (ptr != null && ptr.IsWord) return;

            // Prefix is not in tree.
            if (ptr == null) return;

            // Retrieve auto complete options.
            SuggestionsAux(ptr.Equal, options, temp);
        }

        /// <summary>
        /// Helper auxiliary method to find all prefix suggestion matches.
        /// </summary>
        /// <param name="node">Current node to process.</param>
        /// <param name="options">List of found suggestions.</param>
        /// <param name="buffer">Prefix buffer.</param>
        private void SuggestionsAux(Node node, List<string> options, string buffer)
        {
            if (node == null)
            {
                return;
            }

            // Continue looking in left branch.
            if (node.Less != null) SuggestionsAux(node.Less, options, buffer);

            // Word was found, push into autocomplete options.
            if (node.IsWord)
            {
                options.Add(buffer + node.Value);
            }

            // Continue in middle branch, and push character.
            if (node.Equal != null)
            {
                SuggestionsAux(node.Equal, options, buffer + node.Value);
            }

            // Continue looking in right branch.
            if (node.Greater != null)
            {
                SuggestionsAux(node.Greater, options, buffer);
            }
        }

        /// <summary>
        /// Helper auxiliary function to remove a word.
        /// </summary>
        /// <param name="node">Current node to process.</param>
        /// <param name="word">String to look for and remove.</param>
        /// <returns>True if the node is word.</returns>
        private bool RemoveAux(Node node, string word)
        {
            if (node == null) return false;

            // String is in TST.
            if (word.Length == 1 && node.Value == word[0])
            {
                // Un-mark word node.
                if (node.IsWord)
                {
                    node.IsWord = false;
                    return node.Equal == null && node.Less == null && node.Greater == null;
                }
                else
                {
                    // String is a prefix.
                    return false;
                }
            }
            else
            {
                // String is a prefix.
                if (word[0] < node.Value)
                {
                    RemoveAux(node.Less, word);
                }
                else if (word[0] > node.Value)
                {
                    RemoveAux(node.Greater, word);
                }
                else if (word[0] == node.Value)
                {
                    // String is in TST.
                    // Char is unique.
                    if (RemoveAux(node.Equal, word[1..]))
                    {
                        node.Equal = null;
                        return !node.IsWord && node.Equal == null && node.Less == null && node.Greater == null;
                    }
                }
            }

            return false;
        }
    }
}