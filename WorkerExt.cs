namespace Employee {
    public class WorkerExt : Worker {
        public bool Selected { get; set; }
        public int TaskCount { get; set; }
        public WorkerExt() {
        }
        public WorkerExt(int id, string name, string surname, int count) {
            Id = id; Name = name; Surname = surname; TaskCount = count;
            Selected = false;
        }
    }
}
