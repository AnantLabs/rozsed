using System.Collections.Generic;
using System;

namespace ROZSED.Std
{
    public static class FloydWarshallAlgorithm
    {
        /// <summary>
        /// <para>Returns parent matrix and populate this distance matrix using the Floyd-Warshall algorithm. Populate also diagonal values.</para>
        /// <param name="dist">This array (or first parameter) is distance matrix, where '-1' means disconnected, and '1' connected</param>
        /// </summary>
        public static int[,] FloydWarshall(this int[,] dist)
        {
            int ij, ik, kj, ikj;
            int i, j, k, n = dist.GetLength(0);
            if (n != dist.GetLength(1))
                throw new ArgumentException("Distance matrix must be square.");
            
            //algorithm initialization
            var parent = new int[n, n];
            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                    if (i == j || dist[i, j] == -1) // diagonal case and '-1' in dist matrix means disconnected
                        parent[i, j] = -1;
                    else // other values are connected, so we can pass from vertex to parent matrix
                        parent[i, j] = i;

            // main loops
            for (k = 0; k < n; k++)
                for (i = 0; i < n; i++)
                    for (j = 0; j < n; j++)
                    {
                        if (k == i && i == j) // i = k = j => 'ii' < 'ii' + 'ii'
                            continue;

                        ik = dist[i, k];
                        kj = dist[k, j];
                        if (ik == -1 || kj == -1) // path i => k => j doesn't exist
                            continue;

                        ij = dist[i, j];
                        ikj = ik + kj;

                        // path i => j doesn't exists or
                        // path i => k => j is shorter then i => j, so i => k => j is presently the shortest path between i and j
                        if (ij == -1 || ij > ikj)
                        {
                            dist[i, j] = ikj;
                            parent[i, j] = k;
                        }
                    }
            return parent;
        }
        /// <summary>
        /// <para>Returns parent matrix and populate this distance matrix using the Floyd-Warshall algorithm. Populate also diagonal values.</para>
        /// <param name="dist">This array (or first parameter) is distance matrix, where '-1' means disconnected, and '1' connected</param>
        /// </summary>
        public static int[,] FloydWarshall(this double[,] dist)
        {
            double ij, ik, kj, ikj;
            int i, j, k, n = dist.GetLength(0);
            if (n != dist.GetLength(1))
                throw new ArgumentException("Distance matrix must be square.");

            //algorithm initialization
            var parent = new int[n, n];
            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                    if (i == j || dist[i, j] == -1) // diagonal case and '-1' in dist matrix means disconnected
                        parent[i, j] = -1;
                    else // other values are connected, so we can pass from vertex to parent matrix
                        parent[i, j] = i;

            // main loops
            for (k = 0; k < n; k++)
                for (i = 0; i < n; i++)
                    for (j = 0; j < n; j++)
                    {
                        if (k == i && i == j) // don't replace 'ij' by 'ij' + 'ji'
                            continue;

                        ik = dist[i, k];
                        kj = dist[k, j];
                        if (ik == -1 || kj == -1) // path i => k => j doesn't exist
                            continue;

                        ij = dist[i, j];
                        ikj = ik + kj;

                        // path i => j doesn't exists or
                        // path i => k => j is shorter then i => j, so i => k => j is presently the shortest path between i and j
                        if (ij == -1 || ij > ikj)
                        {
                            dist[i, j] = ikj;
                            parent[i, j] = k;
                        }
                    }
            return parent;
        }
        /// <summary>
        /// Return INTERMEDIATE points in closed and nonclosed path.
        /// </summary>
        public static List<int> GetPath(this int[,] parent, int i, int j)
        {
            if (i == j) // diagonal values => get closed path
                return parent.GetPath(i);
            if (parent[i, j] == -1) // path not exists
                return null;

            var path = new List<int>(); // List is reference type, so we must everytime create new one
            var index = j;
            while ((parent[i, index] != -1) && (parent[i, index] != i))
            {
                path.Add(parent[i, index]);
                index = parent[i, index];
            }
            path.Reverse(); // loop above get vertex in reverse order
            return path;
        }
        /// <summary>
        /// Return INTERMEDIATE points in closed path.
        /// </summary>
        public static List<int> GetPath(this int[,] parent, int i)
        {
            var turn = parent[i, i];
            if (turn == -1)
                return null;

            var path = new List<int>();

            // closed path consists of two nonclosed path (i => turn, turn => i) and turn point (turn)
            path.AddRange(parent.GetPath(i, turn));
            path.Add(turn);
            path.AddRange(parent.GetPath(turn, i));
            return path;
        }
        /// <summary>
        /// Return INTERMEDIATE points in closed path using precalculated nonclosed paths
        /// </summary>
        static List<int> GetPath(this int[,] parent, int i, List<int>[,] paths)
        {
            var turn = parent[i, i];
            if (parent[i, i] == -1) // path not exists
                return null;

            var path = new List<int>();

            // closed path consists of two nonclosed path (i>turn, turn>i) and turn point (turn)
            path.AddRange(paths[i, turn]);
            path.Add(turn);
            path.AddRange(paths[turn, i]);
            return path;
        }
        /// <summary>
        /// <para>Returns array of Lists, which contains indexes of INTERMEDIATE points of path.</para>
        /// <para>Idexes be responding to source IPointCollection.</para>
        /// <para>Path from 'i' over 'j', 'k', 'l' to 'm' is store as follows: list[i, m] = {j, k, l}</para>
        /// <para>Returned array contains also closed path (e.g. from i to i).</para>
        /// <param name="parent">This array (or first parameter) is Parent matrix returned by Floyd-Warshall algorithm.</param>
        /// </summary>
        public static List<int>[,] GetPaths(this int[,] parent)
        {
            int i, j, n = parent.GetLength(0);
            var paths = new List<int>[n, n];

            // calctulate only non-closed path
            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                {
                    if (i == j) // diagonal values will be solved later
                        continue;
                    paths[i, j] = parent.GetPath(i, j);
                }

            // loop for diagonal values (calculate closed path)
            for (i = 0; i < n; i++)
                paths[i, i] = parent.GetPath(i, paths);

            return paths;
        }
        /// <summary>
        /// <para>Returns array of Lists, which contains indexes of INTERMEDIATE points of closed path.</para>
        /// <para>Path from 'i' over 'j', 'k', 'l' to 'i' is store as follows: list[i] = {j, k, l}</para>
        /// <param name="parent">This array (or first parameter) is Parent matrix returned by Floyd-Warshall algorithm.</param>
        /// </summary>
        public static List<int>[] GetClosedPaths(this int[,] parent)
        {
            int i, n = parent.GetLength(0);
            var paths = new List<int>[n];

            // calctulate only closed path
            for (i = 0; i < n; i++)
                paths[i] = parent.GetPath(i);

            return paths;
        }
    }
}
