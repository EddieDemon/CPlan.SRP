// srp4net 1.0
// SRP for .NET - A JavaScript/C# .NET library for implementing the SRP authentication protocol
// http://code.google.com/p/srp4net/
// Copyright 2010, Sorin Ostafiev (http://www.ostafiev.com/)
// License: GPL v3 (http://www.gnu.org/licenses/gpl-3.0.txt)

$u.crypto.srp =
{
    N: '',
    g: '',
    k: '',

    Nbits: 0,

    one: '',
    two: '',
    three: '',

    bigint2hex: function(i)
    {
        // convert from a bigint number to a hex string
        return bigInt2str(i, 16);
    },

    hex2bigint: function(s)
    {
        // convert from a hex string to a bigint number
        return str2bigInt(s, 16, 1);
    },

    H: function(x)
    {
        // hash a message
        return $u.crypto.hash(x);
    },

    HHex: function(x)
    {
        // hash a hex string
        return $u.crypto.hashHex(x);
    },

    AddAccount: function(username, password, fnSuccess)
    {
        // Creating a new account
        // username = the username entered in the interface
        // password = the password entered in the interface

        // user = H(username)
        var user = this.H("!@#<32}|{_$+)EW:>fWS@@!=39dje%^#$RF']]ew3" + username + "shaSHA@"); // some decorations to prevent attacks like the ones based on rainbow tables...
        username = '';

        // pass = H(password)
        var pass = this.H("\":}[\|weFC@de';{{3$$  vdRF2w^5V|\\/.w32" + password + "sshHAA!%"); // some decorations to prevent attacks like the ones based on rainbow tables...
        password = '';

        // s - user's salt
        // s = random number
        var s = randBigInt(this.Nbits, 0);
        var shex = this.bigint2hex(s);

        // x - a private key derived from the password and salt
        // x = H(s || H(username) || H(password))
        var x;
        {
            var x1 = this.hex2bigint(this.HHex(
                shex +
                user +
                pass));
            if (compareTo(x1, this.N) < 0) { x = x1; }
            else { x = mod(x1, sub(this.N, this.one)); }
        }

        // v - the server's verifier
        // v = g^x (mod N)
        var v = powMod(this.g, x, this.N);

        // call the server's AddAccount method
        WS("SRP/AddAccount", {
            user: user,
            s: shex,
            v: this.bigint2hex(v)
        }, function(result)
        {
            fnSuccess(result.error, result.data);
        });
    },

    Authenticate: function(username, password, fnSuccess)
    {
        // Authenticate an user
        // username = the username entered in the interface
        // password = the password entered in the interface

        // user = H(username)
        var user = this.H("!@#<32}|{_$+)EW:>fWS@@!=39dje%^#$RF']]ew3" + username + "shaSHA@"); // some decorations to prevent attacks like the ones based on rainbow tables...
        username = '';

        // pass = H(password)
        var pass = this.H("\":}[\|weFC@de';{{3$$  vdRF2w^5V|\\/.w32" + password + "sshHAA!%"); // some decorations to prevent attacks like the ones based on rainbow tables...
        password = '';

        var hex2bigint = this.hex2bigint;

        // AuthStep1
        var a;
        var A;
        var AHex;
        var uniq1;
        var sHex;
        var BHex;
        var u;
        {
            // a - ephemeral private key
            // a = random between 2 and N-1
            {
                a = randBigInt(this.Nbits, 0);
                if (compareTo(a, this.N) >= 0) { a = mod(a, sub(this.N, this.one)); }
                if (compareTo(a, this.two) < 0) { a = this.two; }
            }

            // A - public key
            // A = g^a (mod N)
            A = powMod(this.g, a, this.N);
            AHex = this.bigint2hex(A);

            // call the server's SRP/AuthStep1 method
            WS("SRP/AuthStep1", {
                user: user,
                A: AHex
            }, function(result)
            {
                if (0 == result.error)
                {
                    with (result)
                    {
                        uniq1 = data.uniq1;
                        sHex = data.s;
                        BHex = data.B;
                        u = hex2bigint(data.u);
                    }
                }
                else
                {
                    alert('SRP/AuthStep1 failed: error = ' + result.error);
                }
            });
        }


        // AuthStep2
        var KHex;
        var m1Hex;
        var uniq2;
        var m2server;
        {
            var B = this.hex2bigint(BHex)

            // x - private key derived from password and the salt
            // x = H(s || H(username) || H(P))
            var x;
            {
                var x1 = this.hex2bigint(this.HHex(
                    sHex +
                    user +
                    pass));
                if (compareTo(x1, this.N) < 0) { x = x1; }
                else { x = mod(x1, sub(this.N, this.one)); }
            }

            // S - common exponential value
            // S = (B - k * g^x) ^ (a + u * x) (mod N)
            var S = powMod(mod(sub(add(B, mult(this.N, this.k)), mult(powMod(this.g, x, this.N), this.k)), this.N), add(mult(x, u), a), this.N);

            // K - the strong cryptographically session key
            // K = H(S)
            var K = this.hex2bigint(this.HHex(this.bigint2hex(S)));
            KHex = this.bigint2hex(K);

            // m1 - client's proof that it has the correct key
            // m1 = H(A, B, K)
            var m1 = this.hex2bigint(this.HHex(
                AHex +
                BHex +
                KHex));
            m1Hex = this.bigint2hex(m1);

            // call the server's SRP/AuthStep2 method
            WS("SRP/AuthStep2", {
                user: user,
                uniq1: uniq1,
                m1: m1Hex
            }, function(result)
            {
                if (0 == result.error)
                {
                    with (result)
                    {
                        uniq2 = data.uniq2;

                        // m2server - server's proof that it has the correct key
                        m2server = hex2bigint(data.m2);
                    }
                }
                else
                {
                    alert('Failure while calling SRP/AuthStep2');
                }
            });
        }

        {
            // m2 - expected server's proof as computed by the client
            // m2 = H(A, m1, K)
            var m2 = this.hex2bigint(this.HHex(
                AHex +
                m1Hex +
                KHex));

            if (0 == compareTo(m2, m2server))
            {
                // the server is trusted
                $.log('Client says: we trust the server');
            }
            else
            {
                alert('Client says: we do NOT trust the server');
                return 1;
            }
        }

        return 0;
    },

    initialize: function(NHex, gHex, kHex)
    {
        this.one = this.hex2bigint("1");
        this.two = this.hex2bigint("2");
        this.three = this.hex2bigint("3");

        this.N = this.hex2bigint(NHex);
        this.Nbits = bitSize(this.N);
        this.g = this.hex2bigint(gHex);
        this.k = this.hex2bigint(kHex);
    },

    demo: function()
    {
        $.debug(true);

        var user = 'user1';
        var password = 'sameverycomplexpassword';
        
        $.log("INFO: Running the sample for account '" + user + "' and password '" + password + "'");
        $.log("INFO: We'll execute two steps: 1. we will create the account; 2. we will authenticate on that account");
        
        $.log("Adding the account");
        this.AddAccount(user, password, function(error, result)
        {
            if (0 == error)
            {
                $.log("The account was successfully added");
            }
            else
            {
                alert('AddAccount failed: error = ' + error + '\nThis should be the normal behavior when executing the second time this example (because the account was created at the first execution)');
            }
        });

        $.log("Authenticating...");
        var err = this.Authenticate('user1', 'sameverycomplexpassword');
        if (0 == err)
        {
            $.log('Everything went OK. The authentication succeeded.');
        }
        else
        {
            alert('Authenticaticate failed: error = ' + err);
        }

        $.log("Done");
    }
}
