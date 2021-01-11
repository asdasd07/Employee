using System.Linq;

namespace Employee {
    public class Database {
        linqClassesDataContext linq;

        public Database() {
            linq = new linqClassesDataContext();
        }

        public IQueryable<WorkerExt> GetWorkers() {
            var workers = from w in linq.Workers
                          select new WorkerExt(w.Id, w.Name, w.Surname,
                              (from t in linq.Tasks
                               where t.WorkId == w.Id
                               select t.Id).Count());
            return workers;
        }

        public IQueryable<Task> GetTasks() {
            var tasks = from t in linq.Tasks
                        select t;
            return tasks;
        }

        public void InsertWorker(string name, string surname) {
            var worker = new Worker { Name = name, Surname = surname };
            InsertWorker(worker);
        }

        public void InsertWorker(Worker worker) {
            if (worker == null || linq.Workers.Any(x => x.Name == worker.Name
                                               && x.Surname == worker.Surname)) {
                return;
            }
            linq.Workers.InsertOnSubmit(worker);
            linq.SubmitChanges();
        }

        public void DeleteWorker(int id) {
            var worker = linq.Workers.SingleOrDefault(x => x.Id == id);
            DeleteWorker(worker);
        }

        public void DeleteWorker(Worker worker) {
            if (worker == null) {
                return;
            }
            var linkedTasks = from t in linq.Tasks
                              where t.WorkId == worker.Id
                              select t;
            linq.Tasks.DeleteAllOnSubmit(linkedTasks);
            linq.Workers.DeleteOnSubmit(worker);
            linq.SubmitChanges();
        }
    }
}
