using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    internal interface IStateValidator
    {
        /// <summary>
        /// A function which can determine
        /// if the state up to a given length
        /// is valid
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="length">The number of chosen elements in the state</param>
        /// <returns></returns>
        bool IsValid(int[] state, int length);
    }
}
