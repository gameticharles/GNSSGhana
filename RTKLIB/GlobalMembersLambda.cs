using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersLambda
    {
        /*------------------------------------------------------------------------------
        * lambda.c : integer ambiguity resolution
        *
        *          Copyright (C) 2007-2008 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] P.J.G.Teunissen, The least-square ambiguity decorrelation adjustment:
        *         a method for fast GPS ambiguity estimation, J.Geodesy, Vol.70, 65-82,
        *         1995
        *     [2] X.-W.Chang, X.Yang, T.Zhou, MLAMBDA: A modified LAMBDA method for
        *         integer least-squares estimation, J.Geodesy, Vol.79, 552-565, 2005
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2007/01/13 1.0 new
        *-----------------------------------------------------------------------------*/


        /* LD factorization (Q=L'*diag(D)*L) -----------------------------------------*/
        internal static int LD(int n, double Q, double[] L, double[] D)
        {
            int i;
            int j;
            int k;
            int info = 0;
            double a;
            double[] A = GlobalMembersRtkcmn.mat(n, n);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(A, Q, sizeof(double) * n * n);
            for (i = n - 1; i >= 0; i--)
            {
                if ((D[i] = A[i + i * n]) <= 0.0)
                {
                    info = -1;
                    break;
                }
                a = Math.Sqrt(D[i]);
                for (j = 0; j <= i; j++)
                {
                    L[i + j * n] = A[i + j * n] / a;
                }
                for (j = 0; j <= i - 1; j++)
                {
                    for (k = 0; k <= j; k++)
                    {
                        A[j + k * n] -= L[i + k * n] * L[i + j * n];
                    }
                }
                for (j = 0; j <= i; j++)
                {
                    L[i + j * n] /= L[i + i * n];
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(A);
            if (info != 0)
            {
                //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the following C++ macro:
                fprintf(stderr, "%s : LD factorization error\n", __FILE__);
            }
            return info;
        }
        /* integer gauss transformation ----------------------------------------------*/
        internal static void gauss(int n, double[] L, double[] Z, int i, int j)
        {
            int k;
            int mu;

            if ((mu = (int)(Math.Floor((L[i + j * n]) + 0.5))) != 0)
            {
                for (k = i; k < n; k++)
                {
                    L[k + n * j] -= (double)mu * L[k + i * n];
                }
                for (k = 0; k < n; k++)
                {
                    Z[k + n * j] -= (double)mu * Z[k + i * n];
                }
            }
        }
        /* permutations --------------------------------------------------------------*/
        internal static void perm(int n, double[] L, double[] D, int j, double del, double[] Z)
        {
            int k;
            double eta;
            double lam;
            double a0;
            double a1;

            eta = D[j] / del;
            lam = D[j + 1] * L[j + 1 + j * n] / del;
            D[j] = eta * D[j + 1];
            D[j + 1] = del;
            for (k = 0; k <= j - 1; k++)
            {
                a0 = L[j + k * n];
                a1 = L[j + 1 + k * n];
                L[j + k * n] = -L[j + 1 + j * n] * a0 + a1;
                L[j + 1 + k * n] = eta * a0 + lam * a1;
            }
            L[j + 1 + j * n] = lam;
            for (k = j + 2; k < n; k++)
                do
                {
                    double tmp_;
                    tmp_ = L[k + j * n];
                    L[k + j * n] = L[k + (j + 1) * n];
                    L[k + (j + 1) * n] = tmp_;
                } while (0);
            for (k = 0; k < n; k++)
                do
                {
                    double tmp_;
                    tmp_ = Z[k + j * n];
                    Z[k + j * n] = Z[k + (j + 1) * n];
                    Z[k + (j + 1) * n] = tmp_;
                } while (0);
        }
        /* lambda reduction (z=Z'*a, Qz=Z'*Q*Z=L'*diag(D)*L) (ref.[1]) ---------------*/
        internal static void reduction(int n, double[] L, double[] D, ref double Z)
        {
            int i;
            int j;
            int k;
            double del;

            j = n - 2;
            k = n - 2;
            while (j >= 0)
            {
                if (j <= k)
                {
                    for (i = j + 1; i < n; i++)
                    {
                        GlobalMembersLambda.gauss(n, L, Z, i, j);
                    }
                }
                del = D[j] + L[j + 1 + j * n] * L[j + 1 + j * n] * D[j + 1];
                if (del + 1E-6 < D[j + 1]) // compared considering numerical error
                {
                    GlobalMembersLambda.perm(n, L, D, j, del, Z);
                    k = j;
                    j = n - 2;
                }
                else
                {
                    j--;
                }
            }
        }
        /* modified lambda (mlambda) search (ref. [2]) -------------------------------*/
        internal static int search(int n, int m, double[] L, double[] D, double[] zs, double[] zn, double[] s)
        {
            int i;
            int j;
            int k;
            int c;
            int nn = 0;
            int imax = 0;
            double newdist;
            double maxdist = 1E99;
            double y;
            double[] S = GlobalMembersRtkcmn.zeros(n, n);
            double[] dist = GlobalMembersRtkcmn.mat(n, 1);
            double[] zb = GlobalMembersRtkcmn.mat(n, 1);
            double[] z = GlobalMembersRtkcmn.mat(n, 1);
            double[] step = GlobalMembersRtkcmn.mat(n, 1);

            k = n - 1;
            dist[k] = 0.0;
            zb[k] = zs[k];
            z[k] = (Math.Floor((zb[k]) + 0.5));
            y = zb[k] - z[k];
            step[k] = ((y) <= 0.0 ? -1.0 : 1.0);
            for (c = 0; c < DefineConstants.LOOPMAX; c++)
            {
                newdist = dist[k] + y * y / D[k];
                if (newdist < maxdist)
                {
                    if (k != 0)
                    {
                        dist[--k] = newdist;
                        for (i = 0; i <= k; i++)
                        {
                            S[k + i * n] = S[k + 1 + i * n] + (z[k + 1] - zb[k + 1]) * L[k + 1 + i * n];
                        }
                        zb[k] = zs[k] + S[k + k * n];
                        z[k] = (Math.Floor((zb[k]) + 0.5));
                        y = zb[k] - z[k];
                        step[k] = ((y) <= 0.0 ? -1.0 : 1.0);
                    }
                    else
                    {
                        if (nn < m)
                        {
                            if (nn == 0 || newdist > s[imax])
                            {
                                imax = nn;
                            }
                            for (i = 0; i < n; i++)
                            {
                                zn[i + nn * n] = z[i];
                            }
                            s[nn++] = newdist;
                        }
                        else
                        {
                            if (newdist < s[imax])
                            {
                                for (i = 0; i < n; i++)
                                {
                                    zn[i + imax * n] = z[i];
                                }
                                s[imax] = newdist;
                                for (i = imax = 0; i < m; i++)
                                {
                                    if (s[imax] < s[i])
                                    {
                                        imax = i;
                                    }
                                }
                            }
                            maxdist = s[imax];
                        }
                        z[0] += step[0];
                        y = zb[0] - z[0];
                        step[0] = -step[0] - ((step[0]) <= 0.0 ? -1.0 : 1.0);
                    }
                }
                else
                {
                    if (k == n - 1)
                        break;
                    else
                    {
                        k++;
                        z[k] += step[k];
                        y = zb[k] - z[k];
                        step[k] = -step[k] - ((step[k]) <= 0.0 ? -1.0 : 1.0);
                    }
                }
            }
            for (i = 0; i < m - 1; i++) // sort by s
            {
                for (j = i + 1; j < m; j++)
                {
                    if (s[i] < s[j])
                        continue;
                    do
                    {
                        double tmp_;
                        tmp_ = s[i];
                        s[i] = s[j];
                        s[j] = tmp_;
                    } while (0);
                    for (k = 0; k < n; k++)
                        do
                        {
                            double tmp_;
                            tmp_ = zn[k + i * n];
                            zn[k + i * n] = zn[k + j * n];
                            zn[k + j * n] = tmp_;
                        } while (0);
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(S);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(dist);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(zb);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(z);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(step);

            if (c >= DefineConstants.LOOPMAX)
            {
                //C++ TO C# CONVERTER TODO TASK: There is no direct equivalent in C# to the following C++ macro:
                fprintf(stderr, "%s : search loop count overflow\n", __FILE__);
                return -1;
            }
            return 0;
        }
        /* lambda/mlambda integer least-square estimation ------------------------------
        * integer least-square estimation. reduction is performed by lambda (ref.[1]),
        * and search by mlambda (ref.[2]).
        * args   : int    n      I  number of float parameters
        *          int    m      I  number of fixed solutions
        *          double *a     I  float parameters (n x 1)
        *          double *Q     I  covariance matrix of float parameters (n x n)
        *          double *F     O  fixed solutions (n x m)
        *          double *s     O  sum of squared residulas of fixed solutions (1 x m)
        * return : status (0:ok,other:error)
        * notes  : matrix stored by column-major order (fortran convension)
        *-----------------------------------------------------------------------------*/
        public static int lambda(int n, int m, double a, double Q, ref double F, ref double s)
        {
            int info;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *L,*D,*Z,*z,*E;
            double L;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *D;
            double D;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Z;
            double Z;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *z;
            double z;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *E;
            double E;

            if (n <= 0 || m <= 0)
            {
                return -1;
            }
            L = GlobalMembersRtkcmn.zeros(n, n);
            D = GlobalMembersRtkcmn.mat(n, 1);
            Z = GlobalMembersRtkcmn.eye(n);
            z = GlobalMembersRtkcmn.mat(n, 1);
            E = GlobalMembersRtkcmn.mat(n, m);

            /* LD factorization */
            if ((info = GlobalMembersLambda.LD(n, Q, L, D)) == 0)
            {

                /* lambda reduction */
                GlobalMembersLambda.reduction(n, L, D, ref Z);
                GlobalMembersRtkcmn.matmul("TN", n, 1, n, 1.0, Z, a, 0.0, ref z); // z=Z'*a

                /* mlambda search */
                if ((info = GlobalMembersLambda.search(n, m, L, D, z, E, s)) == 0)
                {

                    info = GlobalMembersRtkcmn.solve("T", Z, E, n, m, ref F); // F=Z'\E
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(L);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(D);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Z);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(z);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(E);
            return info;
        }
    }
}
