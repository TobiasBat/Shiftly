using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LatinSquares
{
    // https://cs.uwaterloo.ca/~dmasson/tools/latin_square/


    public static int[,] doubleLatinSquare4Indices =  {
        { 0, 1, 3, 2 },
        { 1, 2, 0, 3 },
        { 2, 3, 1, 0 },
        { 3, 0, 2, 1 },

        { 2, 3, 1, 0 },
        { 3, 0, 2, 1 },
        { 0, 1, 3, 2 },
        { 1, 2, 0, 3 }
    };


    // A:0, B:1 C:2 D:3 E:4 F:5
    public static int[,] doubleLatinSquare6Indices =
    {
        { 0, 1, 5, 2, 4, 3 },
        { 1, 2, 0, 3, 5, 4 },
        { 2, 3, 1, 4, 0, 5 },
        { 3, 4, 2, 5, 1, 0 },
        { 4, 5, 3, 0, 2, 1 },
        { 5, 0, 4, 1, 3, 2 },

        { 3, 4, 2, 5, 1, 0 },
        { 4, 5, 3, 0, 2, 1 },
        { 5, 0, 4, 1, 3, 2 },
        { 0, 1, 5, 2, 4, 3 },
        { 1, 2, 0, 3, 5, 4 },
        { 2, 3, 1, 4, 0, 5}
    };


    /** 
     * A,B,F,C,E,D
     * B,C,A,D,F,E
     * C,D,B,E,A,F
     * D,E,C,F,B,A
     * E,F,D,A,C,B
     * F,A,E,B,D,C
     * 
     * A 0
     * B 1
     * C 2
     * D 3
     * E 4
     * F 5
     * 
     **/
    private static int[][] singleLatinSquare6 =
    {
        new []{0,1,5,2,4,3},
        new []{1,2,0,3,5,4},
        new []{2,3,1,4,0,5},
        new []{3,4,2,5,1,0},
        new []{4,5,3,0,2,1},
        new []{5,0,4,1,3,2}
    };

    private static int[][] singleLatinSquare6Transposed =
    {
        new []{0,1,2,3,4,5},
        new []{1,2,3,4,5,0},
        new []{5,0,1,2,3,4},
        new []{2,3,4,5,0,1},
        new []{4,5,0,1,2,3},
        new []{3,4,5,0,1,2}
    };



    public static int[][] singleLatinSquare3x6IndicesRowColReanranged = new[]
    {
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 0, 1, 2),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 1, 2, 3),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 2, 3, 4),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 3, 4, 5),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 4, 5, 0),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 5, 0, 1),
        
      CombineThreeRowsEach6Entrances(singleLatinSquare6Transposed, 0, 1, 2),
      CombineThreeRowsEach6Entrances(singleLatinSquare6Transposed, 1, 2, 3),
      CombineThreeRowsEach6Entrances(singleLatinSquare6Transposed, 2, 3, 4),
      CombineThreeRowsEach6Entrances(singleLatinSquare6Transposed, 3, 4, 5),
      CombineThreeRowsEach6Entrances(singleLatinSquare6Transposed, 4, 5, 0),
      CombineThreeRowsEach6Entrances(singleLatinSquare6Transposed, 5, 0, 1),

      CombineThreeRowsEach6Entrances(singleLatinSquare6, 5, 4, 3),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 4, 3, 2),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 3, 2, 1),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 2, 1, 0),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 1, 0, 5),
      CombineThreeRowsEach6Entrances(singleLatinSquare6, 0, 5, 4)
    };

    private static int[] CombineThreeRowsEach6Entrances(int[][] array, int rowIndex1, int rowIndex2, int rowIndex3)
    {
        int[] result = new int[6 * 3]; 
        for (int i = 0; i < 6; i++)
        {
            result[i] = array[rowIndex1][i]; 
        }
        for (int i = 0; i <6; i++)
        {
            result[6 + i] = array[rowIndex2][i];
        }
        for (int i = 0; i < 6; i++)
        {
            result[2 * 6 + i] = array[rowIndex3][i];
        }

        return result; 
    } 




}
