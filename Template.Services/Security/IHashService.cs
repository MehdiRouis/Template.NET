using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Services.Security
{
    public interface IHashService
    {
        string Hash(string text, string pepperKey);
        bool Verify(string text, string textHash, string pepperKey);
    }
}

