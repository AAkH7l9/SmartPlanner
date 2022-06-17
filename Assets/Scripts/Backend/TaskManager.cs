using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

namespace Backend
{
    public class TaskManager
    {
        private static TaskManager instance;

        public static TaskManager GetInstance(
            List<Task> tasks,
            List<string> blockingTime,
            int idCounter
        )
        {
            if (instance == null)
            {
                instance = new TaskManager(tasks, blockingTime, idCounter);
            }

            return instance;
        }

        public static TaskManager GetInstance()
        {
            return instance;
        }

        private List<Task> _tasks;
        private static int _idCounter;
        private string[] _blockingTime;
        private Task[] _timeline;


        private TaskManager(
            List<Task> tasks,
            List<string> blockingTime,
            int idCounter)
        {
            _tasks = tasks ?? new List<Task>();
            _idCounter = idCounter;
            _blockingTime = blockingTime.ToArray();
        }

        public void SetTaskStatusById(int id, TaskStatus status)
        {
            _tasks.Find(task => task.Id == id).Status = status;
        }

        public void SetBlockingTime(string[] time)
        {
            _blockingTime = time;
            PlayerPrefs.SetString("blockingtime", JsonConvert.SerializeObject(_blockingTime));
        }

        public Task? GetTaskById(int id)
        {
            return _tasks.Find(task => task.Id == id);
        }

        public Task? GetRelevantTaskById(int id)
        {
            return _tasks.Find(task => (task.Id == id && task.Status == TaskStatus.Relevant));
        }

        public Task? GetAwaitingTaskById(int id)
        {
            return _tasks.Find(task => (task.Id == id && task.Status == TaskStatus.Awaiting));
        }

        public Task? GetOverdueTaskById(int id)
        {
            return _tasks.Find(task => (task.Id == id && task.Status == TaskStatus.Overdue));
        }

        public Task? GetDoneTaskById(int id)
        {
            return _tasks.Find(task => (task.Id == id && task.Status == TaskStatus.Done));
        }

        public TimeSpan GetFreeTime(DateTime day)
        {
            UpdateTasks();
            return MagicAlgorithm.FreeTime(_timeline, day);
        }

        public List<Task> GetAllRelevantTasks()
        {
            return _tasks.Where(t => t.Status == TaskStatus.Relevant).ToList();
        }

        public List<Task> GetAllAwaitingTasks()
        {
            return _tasks.Where(t => t.Status == TaskStatus.Awaiting).ToList();
        }

        public List<Task> GetAllOverdueTasks()
        {
            return _tasks.Where(t => t.Status == TaskStatus.Overdue).ToList();
        }

        public List<Task> GetAllDoneTasks()
        {
            return _tasks.Where(t => t.Status == TaskStatus.Done).ToList();
        }

        public int GetOverdueTasksCount()
        {
            int count = GetAllOverdueTasks().Count - PlayerPrefs.GetInt("overduecount");
            PlayerPrefs.SetInt("overduecount", GetAllOverdueTasks().Count);
            return count;
        }

        public void AddTask(Task task)
        {
            task.Id = ++_idCounter;
            PlayerPrefs.SetInt("idcounter", _idCounter);
            //File.WriteAllText("idcounter.txt", _idCounter.ToString());
            _tasks.Add(task);
            UpdateTasks();
        }

        public bool EditTask(Task task)
        {
            int index = _tasks.FindIndex(_task => _task.Id == task.Id);
            if (index == -1)
                return false;
            _tasks[index] = task;
            UpdateTasks();
            return true;
        }

        public bool DeleteTaskById(int id)
        {
            bool deleted = _tasks.Remove(_tasks.Find(task => task.Id == id));
            if (deleted)
                UpdateTasks();
            return deleted;
        }

        public void UpdateTasks()
        {
            Task[] tasksArray = _tasks.ToArray();
            foreach (Task task in tasksArray)
            {
                task.Beginning = task.DataDeadline.AddMinutes(-task.TimeInMinutes);
                task.Ending = task.DataDeadline;
            }
            MagicAlgorithm.RankingByImportance(ref tasksArray);
            MagicAlgorithm.SortingTask(tasksArray);
            _tasks = tasksArray.ToList();
            string taskString = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
            PlayerPrefs.SetString("json", taskString);

            if (!PlayerPrefs.HasKey("overduecount")) PlayerPrefs.SetInt("overduecount", GetAllOverdueTasks().Count);
            //File.WriteAllText("tasks.json", taskString);
        }





        //Основной алгоритм
        //Господи помилуй рефакторить этот код
        public static class MagicAlgorithm
        {

            public static void SortingTask(Task[] listTasks)
            {
                Task[] timeLine;
                Task[] arrayBlockTime = DailyTimeLimit(GetInstance()._blockingTime);
                timeLine = AddBlokingTime(arrayBlockTime, listTasks);

                OffsetTask(ref listTasks, ref timeLine);
                OverdueTasks(listTasks, timeLine);
                GetInstance()._timeline = timeLine;
            }


            private static Task[] AddBlokingTime(Task[] blockTime, Task[] listTasks)
            {
                Task[] timeLine = new Task[listTasks.Length + LengthTimeLIne(listTasks)];

                for (int i = 0; i < LengthTimeLIne(listTasks) + 1; i++)
                {
                    timeLine[i] = new Task();
                    timeLine[i].DataDeadline = blockTime[(i + 6) % 7].DataDeadline;
                    timeLine[i].Beginning = blockTime[(i + 6) % 7].Beginning;
                    timeLine[i].Ending = blockTime[(i + 6) % 7].Ending;
                    timeLine[i].IsFixed = true;

                    blockTime[(i + 6) % 7].DataDeadline = blockTime[(i + 6) % 7].DataDeadline.AddDays(7);
                    blockTime[(i + 6) % 7].Beginning = blockTime[(i + 6) % 7].Beginning.AddDays(7);
                    blockTime[(i + 6) % 7].Ending = blockTime[(i + 6) % 7].Ending.AddDays(7);
                }

                return timeLine;
            }

            private static void OffsetTask(ref Task[] listTasks, ref Task[] timeLine) // переписать название функции
            {
                for (int i = 0; i < listTasks.Length; i++)
                {
                    Task[] temporaryTimeLine = CopyingArrayTasks(timeLine);

                    if (NotEnoughTimeQuestion(ref listTasks[i]))
                    {
                    }
                    else
                    {
                        SearchLocationTask(ref timeLine, temporaryTimeLine, listTasks[i]);
                    }
                }
            }

            private static void SearchLocationTask(ref Task[] timeLine, Task[] temporaryTimeLine, Task task) // разбить её на под функции 
            {
                if (NotEnoughTimeQuestion(ref task)) return;
                for (int j = 0; temporaryTimeLine[j] != null; j++)
                {
                    if (ExecutedFirstQuestion(task, temporaryTimeLine[j]))
                    {
                        if (NotOverlapQuestion(task, temporaryTimeLine[j]))
                        {
                            if ((j == 0) || (NotOverlapQuestion(temporaryTimeLine[j - 1], task)))
                            {
                                timeLine = CopyingArrayTasks(temporaryTimeLine);
                                InsertTask(ref timeLine, task, j);
                                break;
                            }

                            if (temporaryTimeLine[j - 1].IsFixed)
                            {
                                ShiftLocationTask(ref task, ref temporaryTimeLine[j - 1]);
                                SearchLocationTask(ref timeLine, temporaryTimeLine, task);
                                break;
                            }

                            ShiftLocationTask(ref temporaryTimeLine[j - 1], ref task);
                            if (NotEnoughTimeQuestion(ref temporaryTimeLine[j - 1]))
                            {
                                continue;
                            }

                            InsertTask(ref temporaryTimeLine, task, j - 1);

                            task = temporaryTimeLine[j];
                            temporaryTimeLine[j] = null;
                            SearchLocationTask(ref timeLine, temporaryTimeLine, task);
                            break;
                        }

                        ShiftLocationTask(ref task, ref temporaryTimeLine[j]);
                        SearchLocationTask(ref timeLine, temporaryTimeLine, task);
                        break;
                    }

                    if (ItLastTask(temporaryTimeLine, j + 1))
                    {
                        if (NotOverlapQuestion(temporaryTimeLine[j], task))
                        {
                            timeLine = CopyingArrayTasks(temporaryTimeLine);
                            InsertTask(ref timeLine, task, j + 1);
                            break;
                        }

                        if (temporaryTimeLine[j].IsFixed)
                        {
                            ShiftLocationTask(ref task, ref temporaryTimeLine[j]);
                            SearchLocationTask(ref timeLine, temporaryTimeLine, task);
                            break;
                        }

                        ShiftLocationTask(ref temporaryTimeLine[j], ref task);
                        if (NotEnoughTimeQuestion(ref temporaryTimeLine[j]))
                        {
                            continue;
                        }

                        InsertTask(ref temporaryTimeLine, task, j);

                        task = temporaryTimeLine[j + 1];
                        temporaryTimeLine[j + 1] = null;

                        SearchLocationTask(ref timeLine, temporaryTimeLine, task);
                        break;
                    }
                }
            }

            public static bool ItLastTask(Task[] arrayTasks, int index)
            {
                for (int i = index; i + 1 < arrayTasks.Length; i++)
                {
                    if (arrayTasks[i + 1] != null)
                    {
                        return false;
                    }
                }

                return true;
            }

            public static bool NotOverlapQuestion(Task task1, Task task2)
            {
                if (task1.Ending <= (task2?.Beginning ?? DateTime.Now))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static bool NotEnoughTimeQuestion(ref Task task)
            {
                if (task.Beginning < DateTime.Now)
                {
                    task.IsEnoughTime = false;
                    return true;
                }

                return false;
            }

            private static bool ExecutedFirstQuestion(Task task, Task task2)
            {
                if ((task.Ending < (task2?.Ending ?? DateTime.Now)))
                {
                    return true;
                }

                if (task.Ending == (task2?.Ending ?? DateTime.Now))
                {
                    if ((task.Importance >= (task2?.Importance ?? task.Importance + 1)))
                    {
                        return true;
                    }
                }

                return false;
            }

            private static void ShiftLocationTask(ref Task shiftingTask, ref Task fixedTask)
            {
                var timeDifference = -fixedTask.Beginning.Subtract(shiftingTask.Ending);
                shiftingTask.Ending = shiftingTask.Ending.Subtract(timeDifference);
                shiftingTask.Beginning = shiftingTask.Beginning.Subtract(timeDifference);
            }

            private static void InsertTask(ref Task[] timeLine, Task task, int index) // Разбить функцию на под функцию
            {
                bool offset = false;

                if (task.IsEnoughTime == true)
                {
                    Task[] temporaryTimeLine = CopyingArrayTasks(timeLine);

                    for (int i = 0; i < timeLine.Length; i++)
                    {
                        if (i == index)
                        {
                            timeLine[i] = task;
                            offset = true;
                            continue;
                        }

                        if (offset == true)
                        {
                            timeLine[i] = temporaryTimeLine[i - 1];
                            continue;
                        }

                        if ((i + 1 == timeLine.Length) || (timeLine[i] == null))
                        {
                            break;
                        }
                    }
                }
            }

            public static Task[] CopyingArrayTasks(Task[] arrayTasks)
            {
                Task[] newArrayTasks = new Task[arrayTasks.Length];
                for (int i = 0; i < arrayTasks.Length; i++)
                {
                    if (arrayTasks[i] != null)
                    {
                        newArrayTasks[i] = new Task(
                            arrayTasks[i]?.Name,
                            arrayTasks[i].TimeInMinutes,
                            arrayTasks[i].DataDeadline,
                            arrayTasks[i].Importance
                            );
                        newArrayTasks[i].IsEnoughTime = arrayTasks[i].IsEnoughTime;
                        newArrayTasks[i].IsFixed = arrayTasks[i].IsFixed;
                    }
                }

                return newArrayTasks;
            }

            private static Task[] DailyTimeLimit(string[] arrayLimiyTime)
            {
                Task[] arrayBlokedTime = new Task[arrayLimiyTime.Length / 2];
                for (int i = 0; i < arrayBlokedTime.Length; i++)
                {
                    Task blockedTime = new Task();

                    blockedTime.Beginning = DateTime.Today.AddDays(i - 1);
                    blockedTime.DataDeadline = DateTime.Today.AddDays(i);

                    if (blockedDayWeek(i) == 0)
                    {
                        blockedTime.Beginning =
                            blockedTime.Beginning.AddHours((Convert.ToDateTime(arrayLimiyTime[13])).Hour);
                        blockedTime.Beginning =
                            blockedTime.Beginning.AddMinutes((Convert.ToDateTime(arrayLimiyTime[13])).Minute);
                    }
                    else
                    {
                        blockedTime.Beginning =
                            blockedTime.Beginning.AddHours((Convert.ToDateTime(arrayLimiyTime[blockedDayWeek(i) * 2 - 1]))
                                .Hour);
                        blockedTime.Beginning =
                            blockedTime.Beginning.AddMinutes((Convert.ToDateTime(arrayLimiyTime[blockedDayWeek(i) * 2 - 1]))
                                .Minute);
                    }

                    if (i == 0)
                    {
                        Task lastBlockedTime = new Task();
                        lastBlockedTime.DataDeadline =
                            lastBlockedTime.DataDeadline.AddHours((Convert.ToDateTime(arrayLimiyTime[12])).Hour);
                        lastBlockedTime.DataDeadline =
                            lastBlockedTime.DataDeadline.AddMinutes((Convert.ToDateTime(arrayLimiyTime[12])).Minute);
                        if (blockedTime.Beginning.Hour < lastBlockedTime.DataDeadline.Hour)
                        {
                            blockedTime.Beginning = blockedTime.Beginning.AddDays(1);
                        }

                        if (blockedTime.Beginning.Hour == lastBlockedTime.DataDeadline.Hour)
                        {
                            if (blockedTime.Beginning.Minute < lastBlockedTime.DataDeadline.Minute)
                            {
                                blockedTime.Beginning = blockedTime.Beginning.AddDays(1);
                            }
                        }
                    }
                    else
                    {
                        if (blockedTime.Beginning.Hour < arrayBlokedTime[i - 1].DataDeadline.Hour)
                        {
                            blockedTime.Beginning = blockedTime.Beginning.AddDays(1);
                        }

                        if (blockedTime.Beginning.Hour == arrayBlokedTime[i - 1].DataDeadline.Hour)
                        {
                            if (blockedTime.Beginning.Minute < arrayBlokedTime[i - 1].DataDeadline.Minute)
                            {

                                blockedTime.Beginning = blockedTime.Beginning.AddDays(1);
                            }
                        }
                    }

                    blockedTime.DataDeadline =
                        blockedTime.DataDeadline.AddHours((Convert.ToDateTime(arrayLimiyTime[blockedDayWeek(i) * 2])).Hour);
                    blockedTime.DataDeadline =
                        blockedTime.DataDeadline.AddMinutes((Convert.ToDateTime(arrayLimiyTime[blockedDayWeek(i) * 2]))
                            .Minute);

                    if (i == 6)
                    {
                        blockedTime.Beginning = blockedTime.Beginning.AddDays(-7);
                        blockedTime.DataDeadline = blockedTime.DataDeadline.AddDays(-7);
                    }

                    BlockedTimeInOneDay(ref blockedTime);

                    blockedTime.Ending = blockedTime.DataDeadline;
                    blockedTime.IsFixed = true;

                    arrayBlokedTime[i] = blockedTime;
                }

                return arrayBlokedTime;
            }

            private static int blockedDayWeek(int i)
            {
                int index = 0;
                if (((int)DateTime.Today.DayOfWeek + 13) % 7 + i > 6)
                {
                    index = ((int)DateTime.Today.DayOfWeek + 13) % 7 + i - 7;
                }
                else
                {
                    index = ((int)DateTime.Today.DayOfWeek + 13) % 7 + i;
                }

                return index;
            }

            private static void BlockedTimeInOneDay(ref Task blockedTime)
            {
                if (blockedTime.DataDeadline < blockedTime.Beginning)
                {
                    blockedTime.Beginning = blockedTime.Beginning.AddDays(-1);
                }
            }

            private static int LengthTimeLIne(Task[] listTask)
            {
                RankingByDeadLine(ref listTask);

                int Difference = (int)(listTask[0].DataDeadline - DateTime.Now.AddDays(-2)).TotalDays;
                if (Difference < 0)
                {
                    Difference = 0;
                }

                return Difference;
            }

            public static Task[] RankingByImportance(ref Task[] listTasks)
            {
                var orderedList = from task in listTasks
                                  orderby task.Importance descending
                                  select task;

                int i = 0;
                foreach (Task task in orderedList)
                {
                    listTasks[i] = task;
                    i++;
                }

                return listTasks;
            }

            public static Task[] RankingByDeadLine(ref Task[] listTasks)
            {
                var orderedList = from task in listTasks
                                  orderby task.DataDeadline descending
                                  select task;

                int i = 0;
                foreach (Task task in orderedList)
                {
                    listTasks[i] = task;
                    i++;
                }

                return listTasks;
            }

            public static Task[] OverdueTasks(Task[] listTasks, Task[] timeline)
            {
                int[] arrayIdTasks = new int[timeline.Length];
                for (int i = 0; i < timeline.Length; i++)
                {
                    arrayIdTasks[i] = timeline[i]?.Id ?? 0;
                }
                for (int i = 0; i < listTasks.Length; i++)
                {
                    if (Array.IndexOf(arrayIdTasks, listTasks[i].Id) < 0)
                    {
                        listTasks[i].IsEnoughTime = false;
                        listTasks[i].Status = TaskStatus.Overdue;
                    }
                }
                return listTasks;
            }

            public static TimeSpan FreeTime(Task[] timeLine, DateTime day)
            {
                TimeSpan freeTime;
                int i = 0;

                while (timeLine[i].Beginning < DateTime.Now)
                {
                    i++;
                }
                if (i == 0)
                {
                    freeTime = timeLine[i].Beginning - DateTime.Now;
                }
                else
                {
                    if (timeLine[i - 1].Ending > DateTime.Now)
                    {
                        freeTime = timeLine[i].Beginning - timeLine[i - 1].Ending;
                    }
                    else
                    {
                        freeTime = timeLine[i].Beginning - DateTime.Now;
                    }
                }

                while (timeLine[i]?.Ending < day.AddDays(1) && i != timeLine.Length - 1)
                {
                    if (timeLine[i] != null && timeLine[i + 1] != null)
                    {
                        freeTime += timeLine[i + 1].Beginning - timeLine[i].Ending;
                    }
                    i++;
                }

                return freeTime;
            }

        }
    }
}

