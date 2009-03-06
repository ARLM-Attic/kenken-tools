using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    class PermutationPrefixTrie
    {
        private int numChildren;
        private PermutationPrefixTrieNode root;

        public PermutationPrefixTrie(int numChildren)
        {
            this.numChildren = numChildren;
            root = new PermutationPrefixTrieNode(false, numChildren);
        }

        public void SetPath(int[] sequence, int length)
        {
            PermutationPrefixTrieNode currNode = root;
            for (int i = 0; i < length; i++)
            {
                if (currNode.end)
                    throw new ArgumentException("setting a path that should not be set");
                PermutationPrefixTrieNode childnode = currNode.children[sequence[i]];
                if (childnode != null)
                {
                    currNode = childnode;
                    if (i == length - 1)
                    {
                        currNode.end = true;
                        currNode.children = null;
                    }
                }
                else
                {
                    if (i == length - 1)
                        currNode.children[sequence[i]] = new PermutationPrefixTrieNode(true, numChildren);
                    else
                    {
                        currNode.children[sequence[i]] = new PermutationPrefixTrieNode(false, numChildren);
                        currNode = currNode.children[sequence[i]];
                    }
                }
            }

        }

        public bool HasPath(int[] sequence, int length)
        {
            PermutationPrefixTrieNode currNode = root;
            for (int i = 0; i < length; i++)
            {
                PermutationPrefixTrieNode childnode = currNode.children[sequence[i]];
                if (childnode == null)
                    return false;
                currNode = childnode;
                if (currNode.end)
                    return true;
            }

            //on a path, but not specific enough yet
            return false;
        }
    }

    internal class PermutationPrefixTrieNode
    {
        public bool end;
        public PermutationPrefixTrieNode[] children;
        public PermutationPrefixTrieNode(bool isEnd, int numChildren)
        {
            this.end = isEnd;
            if (!end)
                children = new PermutationPrefixTrieNode[numChildren];
            //else it will be null
        }

    }
}
