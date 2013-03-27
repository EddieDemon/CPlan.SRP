// srp4net 1.0
// SRP for .NET - A JavaScript/C# .NET library for implementing the SRP authentication protocol
// http://code.google.com/p/srp4net/
// Copyright 2010, Sorin Ostafiev (http://www.ostafiev.com/)
// License: GPL v3 (http://www.gnu.org/licenses/gpl-3.0.txt)

var $u =
{
};

$u.crypto =
{
    sha512: sha.create("SHA-512"),

    hash: function(message)
    {
        // hash a message
        return base16_encode(this.sha512.hash(str2utf8(message)));
    },
    
    hashHex: function(hexmessage)
    {
        // hash a hex string
        return base16_encode(this.sha512.hash(base16_decode(((0 == hexmessage.length % 2) ? '' : '0') + hexmessage)));
    }
}

