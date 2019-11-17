using System;
using System.Linq;

namespace Api.Helpers
{
    /// <summary>
    /// Implements SHA3 encryption algorithm.
    /// Do NOT change anything in this class ever!
    /// </summary>
    internal sealed class Keccak
	{
		private ulong[] RC ={0x0000000000000001, 0x0000000000008082, 0x800000000000808A, 0x8000000080008000, 0x000000000000808B,
			0x0000000080000001, 0x8000000080008081, 0x8000000000008009, 0x000000000000008A, 0x0000000000000088,
			0x0000000080008009, 0x000000008000000A, 0x000000008000808B, 0x800000000000008B, 0x8000000000008089,
			0x8000000000008003, 0x8000000000008002, 0x8000000000000080, 0x000000000000800A, 0x800000008000000A,
			0x8000000080008081, 0x8000000000008080, 0x0000000080000001, 0x8000000080008008};

		private int[,] r = { { 0, 36, 3, 41, 18 }, { 1, 44, 10, 45, 2 }, { 62, 6, 43, 15, 61 }, { 28, 55, 25, 21, 56 }, { 27, 20, 39, 8, 14 } };

		private int w;
		private int l;
		private int n;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keccak"/> class.
        /// Do NOT change anything in this class ever!
        /// </summary>
        /// <param name="b">
        /// B = 1600 for SHA3 256.
        /// </param>
        public Keccak(int b)
		{
			w = b / 25;
			l = Convert.ToInt32(Math.Log(w, 2));
			n = 12 + 2 * l;
		}

		private ulong rot(ulong x, int n)
		{
			n = n % w;
			return (((x << n) | (x >> (w - n))));
		}

		private ulong[,] roundB(ulong[,] a, ulong rc)
		{
			ulong[] C = new ulong[5];
			ulong[] D = new ulong[5];
			ulong[,] B = new ulong[5, 5];

			for (int i = 0; i < 5; i++)
			{
				C[i] = a[i, 0] ^ a[i, 1] ^ a[i, 2] ^ a[i, 3] ^ a[i, 4];
			}

			for (int i = 0; i < 5; i++)
			{
				D[i] = C[(i + 4) % 5] ^ rot(C[(i + 1) % 5], 1);
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					a[i, j] = a[i, j] ^ D[i];
				}
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					B[j, (2 * i + 3 * j) % 5] = rot(a[i, j], r[i, j]);
				}
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					a[i, j] = B[i, j] ^ ((~B[(i + 1) % 5, j]) & B[(i + 2) % 5, j]);
				}
			}

			a[0, 0] = a[0, 0] ^ rc;

			return a;
		}

		private ulong[,] Keccackf(ulong[,] A)
		{
			for (int i = 0; i < n; i++)
			{
				A = roundB(A, RC[i]);
			}

			return A;
		}

		private ulong[][] padding(string m, int r)
		{
			int size = 0;

			m = m + "01";

			while (((m.Length / 2) * 8 % r) != ((r - 8)))
			{
				m = m + "00";
			}

			m = m + "80";

			size = ((m.Length / 2) * 8) / r;

			ulong[][] arrayM = new ulong[size][];
			arrayM[0] = new ulong[1600 / w];
			string temp = "";
			int count = 0;
			int j = 0;
			int i = 0;

			foreach (char ch in m)
			{
				if (j > (r / w - 1))
				{
					j = 0;
					i++;
					arrayM[i] = new ulong[1600 / w];
				}

				count++;

				if ((count * 4 % w) == 0)
				{
					arrayM[i][j] = Convert.ToUInt64(m.Substring((count - w / 4), w / 4), 16);
					temp = ToReverseHexString(arrayM[i][j]);
					arrayM[i][j] = Convert.ToUInt64(temp, 16);
					j++;
				}

			}

			return arrayM;
		}

		private string ToReverseHexString(ulong S)
		{
			return BitConverter.ToString(BitConverter.GetBytes(S).ToArray()).Replace("-", "");
		}

		private string ToHexString(ulong S)
		{
			return BitConverter.ToString(BitConverter.GetBytes(S).Reverse().ToArray()).Replace("-", "");
		}

		public string GetHash(string M, int r, int c, int d)
		{
			ulong[,] S = new ulong[5, 5];

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					S[i, j] = 0;
				}
			}

			ulong[][] P = padding(M, r);

			foreach (ulong[] Pi in P)
			{
				{
					for (int i = 0; i < 5; i++)
					{
						for (int j = 0; j < 5; j++)
						{
							if ((i + j * 5) < (r / w))
							{
								S[i, j] = S[i, j] ^ Pi[i + j * 5];
							}
						}
					}
				}

				Keccackf(S);
			}

			string Z = "";

			do
			{
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						if ((5 * i + j) < (r / w))
						{
							Z = Z + ToReverseHexString(S[j, i]);
						}
					}
				}

				Keccackf(S);
			}
			while (Z.Length < d * 2);

			return Z.Substring(0, d * 2);
		}
	}
}