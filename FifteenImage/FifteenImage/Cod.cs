#region Using

using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.Threading;

#endregion
namespace FifteenImage
{
    //Класс управляющий работой алгоритмом IDA*  
        internal sealed class IDA : SearchBase
        {
            #region Constructor
            public IDA(int[] start)
            {
            //Начальное состояние
            _start = start;
            }
            #endregion
            #region Variables
            //Массив описывающий стартовое состояние, считывание данных 
            //Производится сверху вниз, слева на право
            public int[] _start;
            //Переменная подсчитывающая кол-во обработанных состояний
            public static ulong CountStates;
            //Переменная текущую глубину поиска 
            public static int DepthDistributeStep;
            //Установить время отсчёта
            readonly DateTime _timeStart = DateTime.Now;
            #endregion
            #region Classes
            //Структура описывающая перемещение (изменение состояния)
            struct Move
            {
                public int m_x, m_y;
                public Move(int x, int y)
                {
                    m_x = x;
                    m_y = y;
                }
            }
            //Класс описывающий состояние объекта , 
            //в данном случае позицию фишек игры 15 
            class State
            {
                //Матрица значений фишек
                public uint[,] m_cells;
                //Позиция пустышки
                public int m_emptyX, m_emptyY;
                //Рекурсивная функция проверки является ли данное состояние целевым
                public bool IsFinal(int x, int y, int value)
                {
                    return x == -1 ||
                        m_cells[x, y] == value &&
                        IsFinal(y == 0 ? (x - 1) : x, y == 0 ?
                        Dimension - 1 : (y - 1), --value);
                }
                //Проверка является ли данное состояние целевым
                public bool IsFinalPosition()
                {
                    return IsFinal(Dimension - 1, Dimension - 2, Dimension * Dimension - 2);
                }
                //Функция устанавливающая значение ячейки по её координатам
                public void SetCell(int x, int y, int value)
                {
                    m_cells[x, y] = (uint)(value - 1);
                }
                //Функция устанавливающая положение пустышки
                public void SetEmpty(int x, int y)
                {
                    m_emptyX = x;
                    m_emptyY = y;
                }
                //Функция перемещения
                public uint[,] Move(Move move)
                {
                    m_cells[m_emptyX, m_emptyY] = m_cells[move.m_x, move.m_y];
                    m_cells[move.m_x, move.m_y] = (uint)(Dimension * Dimension);
                    m_emptyX = move.m_x;
                    m_emptyY = move.m_y;
                    return m_cells;
                }
                //Конструктор
                public State()
                {
                    m_cells = new uint[Dimension, Dimension];
                }
            }
           class SearchIDA
            {
                //переменная для хранения текущих состояний
                public State _state = new State();
                //список перемещений из начального состояния в целевое , 
                //т.е. решение которое предстоит найти
                public List<Move> _solution = new List<Move>();
                //переменная которая используется для выхода из локального экстремума 
                //путём увеличения глубины из текущий точки до тех по пока 
                //не будет найденно лучшее продолжение 
                int _nStepDepth;
                #region Functions
                /// <summary>
                /// Основная функция поиска оптимального пути
                /// </summary>
                /// <returns>List<Move> Возвратит список соответвующий оптимальному пути</returns>
                public List<Move> GetOptimalSolution()
                {
                    DepthDistributeStep = 0;
                    if (Dimension - 1 == _state.m_emptyX &&
                        _state.m_emptyX == _state.m_emptyY &&
                        _state.IsFinalPosition())//Если начальное состояние является целью
                        return new List<Move>();//то вернём пустой список шагов пути
                    //используется при попадании алгоритма в точку локального максимума , 
                    //и определяет глубину поиска для выхода из локального максимума 
                    _nStepDepth = 0;
                    int depthDistributeStep = 0;
                    //начнём поиск 
                    if (!DistributeStep(_state.m_emptyX, _state.m_emptyY, 0, 0, true, depthDistributeStep))
                        //мы попали в локальный экстремум  , начнём поиск выхода с 
                        //текущей относительной глубины _nStepDepth + 1 
                        while (!DistributeStep(_state.m_emptyX,
                            _state.m_emptyY, 0, 0, false, depthDistributeStep))
                            //будем увеличивать глубину пока не выйдем из локального экстремума
                            _nStepDepth++;
                    return _solution;
                }
                /// <summary>
                /// Распределяет возможные шаги , в случае игры 15 мы 
                /// имеем четыре возможных продолжения
                /// Система координат для определения позиции ячейки
                ///   0 1 2 3
                ///   - - - - Y
                /// 0|
                /// 1|
                /// 2|
                /// 3|
                ///  X
                /// </summary>
                /// <param name="emptyX">Положение пустышки по оси X</param>
                /// <param name="emptyY">Положение пустышки по оси Y</param>
                /// <param name="emptyXOffset">Перемещение пустышки вдоль оси X</param>
                /// <param name="emptyYOffset">Перемещение пустышки вдоль оси Y</param>
                /// <param name="edge">false - алгоритм попал в локальный максимум</param>
                /// <returns>true - предидущий шаг является путём к решению  , false не является</returns>
                bool DistributeStep(int emptyX, int emptyY,
                    int emptyXOffset, int emptyYOffset, bool edge, int depthDistributeStep)
                {
                    depthDistributeStep++;
                    if (depthDistributeStep > DepthDistributeStep)
                        DepthDistributeStep = depthDistributeStep;
                    //В игре "15" возможны 4 варианта перемещения фишки на пустое место
                    //Если перемещение пустышки вправо возможно 
                    if (emptyY != Dimension - 1 &&
                             -1 != emptyYOffset && //и если мы не делаем возвратное перемещение
                             DoMakeStep(emptyX, emptyY, 0, 1, edge, depthDistributeStep)) //то сделаем его 
                    {
                        //данное перемещение привело к цели или является 
                        //перемещением ведущим к цели , 
                        //т.е. является одним из шагов найденного пути , добавим 
                        //этот шаг к нашему списку  
                        _solution.Add(new Move(emptyX, emptyY + 1));
                        return true;
                    }
                    if (emptyY != 0 && //Если перемещение пустышки влево возможно 
                        1 != emptyYOffset && //и если мы не делаем возвратное перемещение
                        DoMakeStep(emptyX, emptyY, 0, -1, edge, depthDistributeStep))//то сделаем его
                    {
                        _solution.Add(new Move(emptyX, emptyY - 1));
                        return true;
                    }
                    if (emptyX != Dimension - 1 && //Если перемещение пустышки вниз возможно 
                        -1 != emptyXOffset && //и если мы не делаем возвратное перемещение
                        DoMakeStep(emptyX, emptyY, 1, 0, edge, depthDistributeStep))//то сделаем его
                    {
                        _solution.Add(new Move(emptyX + 1, emptyY));
                        return true;
                    }
                    if (emptyX != 0 && //Если перемещение пустышки вверх возможно 
                        1 != emptyXOffset && //и если мы не делаем возвратное перемещение
                        DoMakeStep(emptyX, emptyY, -1, 0, edge, depthDistributeStep))//то сделаем его
                    {
                        _solution.Add(new Move(emptyX - 1, emptyY));
                        return true;
                    }
                    return false;
                }
                /// <summary>
                /// Проверяет не ухудшает ли положение ячейки данное перемещение
                /// </summary>
                /// <param name="cell">содержимое ячейки в новом положении пустышки</param>
                /// <param name="emptyX">Положение пустышки по оси X</param>
                /// <param name="emptyY">Положение пустышки по оси Y</param>
                /// <param name="emptyXOffset">Перемещение пустышки вдоль оси X</param>
                /// <param name="emptyYOffset">Перемещение пустышки вдоль оси Y</param>
                /// <returns></returns>
                static bool IsGoodMove(uint cell //содержимое ячейки в новом положении пустышки
                    , int emptyX, int emptyY, int emptyXOffset, int emptyYOffset)
                {

                    if (emptyXOffset != 0) //мы производим перемещения по оси X
                    {
                        int posMustBe = (int)(cell / Dimension);//целевая позиция ячейки
                        //ячейка займёт своё целевое место при перемещении на место 
                        //пустышки (или при перемещении пустышки на место ячейки)
                        if (posMustBe == emptyX)
                            return true;//возвратим истину
                        if (emptyX < posMustBe)//если целевая позиция ячейки находится вверху
                            //то вернём истину если мы производим перемещение 
                            //пыстышки вниз (=> ячейки вверх) , т.е. мы стремимся к цели
                            return emptyXOffset < 0;
                        return emptyXOffset > 0;
                    }
                    else //мы производим перемещения по оси Y
                    {
                        int posMustBe = (int)(cell % Dimension);//целевая позиция ячейки
                        //ячейка займёт своё целевое место при перемещении на место пустышки 
                        //(или при перемещении пустышки на место ячейки)
                        if (posMustBe == emptyY)
                            return true;//возвратим истину
                        if (emptyY < posMustBe)//если целевая позиция ячейки находится справа
                            //то вернём истину если мы производим перемещение пыстышки влево 
                            //(=> ячейки вправо) , т.е. мы стремимся к цели
                            return emptyYOffset < 0;
                        return emptyYOffset > 0;
                    }
                }
                /// <summary>
                /// Выполняет щаг
                /// </summary>
                /// <param name="emptyX">Положение пустышки по оси X</param>
                /// <param name="emptyY">Положение пустышки по оси Y</param>
                /// <param name="emptyXOffset">Перемещение пустышки вдоль оси X</param>
                /// <param name="emptyYOffset">Перемещение пустышки вдоль оси Y</param>
                /// <param name="edge">false - алгоритм попал в локальный максимум</param>
                /// <returns>true - шаг является путём к решению  , false не является</returns>
                bool DoMakeStep(int emptyX, int emptyY, int emptyXOffset,
                    int emptyYOffset, bool edge, int depthDistributeStep)
                {
                    //новое положение пустышки
                    int newEmptyX = emptyX + emptyXOffset;
                    int newEmptyY = emptyY + emptyYOffset;
                    //содержимое ячейки в новом положении пустышки
                    uint cell = _state.m_cells[newEmptyX, newEmptyY];
                    //улучшает ли данное перемещение пустышки положение ячейки
                    bool isGoodMove = IsGoodMove(cell, emptyX, emptyY, emptyXOffset, emptyYOffset);
                    if (isGoodMove)
                    {
                        CountStates++;
                        //если перемещение пустышки не ухудшает положение ячейки то запоминаем перемещение
                        _state.m_cells[emptyX, emptyY] = cell;
                        if (edge &&//мы находимся в состоянии поиска с улучшающимися положениями 
                            Dimension - 1 == newEmptyX &&//если пустышка на месте по оси X
                            Dimension - 1 == newEmptyY &&//если пустышка на месте по оси Y
                            _state.IsFinalPosition())//если новое положение ячейки приводит к цели 
                        {
                            //то возвратим true - цель найденна 
                            return true;
                        }
                        //если перемещение пустышки улучшает положение ячейки ,
                        //то повторяем операцию перемещения для нового положения пустышки
                        if (DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                            emptyYOffset, edge, depthDistributeStep))
                        {
                            //данное перемещение привело к цели или является перемещением которое ведёт 
                            //к цели , т.е. является одним из шагов найденного пути , вернём истину  
                            return true;
                        }
                        //если все 4 возможных перемещения ухудшают состояние то запоминаем новое 
                        //положение пустышки и выходим с false
                        _state.m_cells[newEmptyX, newEmptyY] = cell;
                        return false;
                    }
                    //Находимся ли мы в состоянии поиска выхода из локального экстремума 
                    if (!edge)
                    {
                        CountStates++;
                        //Данное перемещение неудачно ,но мы исчерпали все 
                        //варианты удачных перемещений 
                        //поэтому делаем данное перемещение
                        _state.m_cells[emptyX, emptyY] = cell;
                        --_nStepDepth;//уменьшаем порог любых продолжений , 
                        //в случае ,если он не отрицателен ,
                        //мы будем изучать все возможные продолжения для выхода 
                        //из локального экстремума 
                        if (_nStepDepth >= 0
                            ? DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                                emptyYOffset, false, depthDistributeStep)
                            : DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                                emptyYOffset, true, depthDistributeStep))
                        {
                            //данное перемещение привело к цели или является 
                            //перемещением которое ведёт 
                            //к цели , т.е. является одним из шагов 
                            //найденного пути , вернём истину  
                            return true;
                        }
                        //преидущая глубина поиска выхода из локального экстремума 
                        //не дала результата , увеличим глубину 
                        ++_nStepDepth;
                        //запоминаем новое положение пустышки и выходим с false
                        _state.m_cells[newEmptyX, newEmptyY] = cell;
                        return false;
                    }
                    return false;
                }
                #endregion
            }
            #endregion
            #region Functions
            //Каждые 0.2 сек выводит текущую статистику работы алгоритма
            protected override void ShowStatistics()
            {
                while (Thread != null && Thread.IsAlive)//(arResult == null)
                {
                    TimeSpan timeSearch = DateTime.Now - _timeStart;
                    Progressing(
                        "Текущее кол-во исследованных состояний\t: " + CountStates.ToString(CultureInfo.InvariantCulture) +
                        Environment.NewLine + "Время поиска\t\t\t\t: " + timeSearch +
                        Environment.NewLine + "Текущая максимальная глубина поиска \t: " + DepthDistributeStep.ToString(CultureInfo.InvariantCulture));
                    Thread.Sleep(200);
            }
            }
            //Проверим может ли данное начальное состояние быть приведённым к целевому
            //Только нечётное кол-во беспорядков в матрице имеет решение
            public static bool DoHaveResolve(int[] ar)
            {
                int ch = 0;
                for (int i = 0; i < ar.Length; i++)
                    for (int j = 0; j < ar.Length; j++)
                    {
                        //Исключим дырку при подсчёте беспорядков
                        if (ar[i] == ar.Length - 1 || ar[j] == ar.Length - 1)
                            continue;
                        if (i < j && ar[i] > ar[j])
                            ch++;
                    }
                //Если сторона матрицы чётная , надо учесть положение дырки
                if (Dimension % 2 == 0)
                    for (int i = 0; i < ar.Length; i++)
                        if (ar[i] == ar.Length - 1)
                        {
                            int row = 0;
                            for (int h = i; h >= 0; h -= Dimension) row++;
                            if (row % 2 == 1)
                                ch++;
                        }
                return (ch % 2) == 0;
            }
            //Основная функция запускающая поиск 
            protected override void StartSearch()
            {
                Started();
                {
                    SearchIDA searchIDA = new SearchIDA();
                    State state = new State();
                    for (int i = 0; i < Dimension * Dimension; ++i)
                    {
                        int value = _start[i];
                        value++;
                        if (value != Dimension * Dimension)
                            state.SetCell(i / Dimension, i % Dimension, value);
                        else
                            state.SetEmpty(i / Dimension, i % Dimension);
                    }
                    searchIDA._state.m_cells = (uint[,])state.m_cells.Clone();
                    searchIDA._state.m_emptyX = state.m_emptyX;
                    searchIDA._state.m_emptyY = state.m_emptyY;
                    CountStates = 0;
                    //Начнём поиск оптимального пути
                    List<Move> solution = searchIDA.GetOptimalSolution();
                    //Мы получили набор перемещений из целевой позиции в начальную
                    solution.Reverse();
                    ArResult = new int[solution.Count + 1][];
                    ArResult[0] = _start;
                    int arIndex = 1;
                    foreach (Move m in solution)
                    {
                        state.Move(m);
                        ArResult[arIndex] = new int[Dimension * Dimension];
                        int j = 0;
                        for (int ii = 0; ii < Dimension; ii++)
                            for (int jj = 0; jj < Dimension; jj++, j++)
                            {
                                if (ii == state.m_emptyX &&
                                    jj == state.m_emptyY)
                                    ArResult[arIndex][j] = Dimension * Dimension - 1;
                                else
                                    ArResult[arIndex][j] = (int)state.m_cells[ii, jj];// - 1;
                            }
                        arIndex++;
                }
                    TimeSpan timeSearch = DateTime.Now - _timeStart;
                    if (ArResult.Length - 1 != 0)
                    {
                        int v=(ArResult.Length - 1)%10;
                        string st = (v == 2 || v == 3 || v == 4) ? " хода" : ((v == 1) ? " ход" : " ходов");
                        Progressing("ИГРА",
                            "Нашёл. " + Environment.NewLine + (ArResult.Length).ToString(CultureInfo.InvariantCulture) +
                            st+Environment.NewLine + "Время:" + timeSearch +
                            Environment.NewLine + "\nОбщее кол-во исследованных состояний\t: " +
                            CountStates.ToString(CultureInfo.InvariantCulture) +
                            Environment.NewLine + "\n**** Поиск завершён ****" +
                            Environment.NewLine + "\nВремя поиска\t: " + timeSearch +
                            Environment.NewLine + "\nКол-во шагов для сбора матрицы\t: " +
                            (ArResult.Length ).ToString(CultureInfo.InvariantCulture));
                    Global.globalCountStates = Convert.ToInt32(CountStates);
                    //Global.globalCountStates = Convert.ToInt32(CountStates);
                    //Global.globalmoves = Convert.ToInt32(ArResult.Length);
                }
                    else
                        Progressing("И что я должен искать ?");
                }
            Finished();
            }
            #endregion
        }

     
}