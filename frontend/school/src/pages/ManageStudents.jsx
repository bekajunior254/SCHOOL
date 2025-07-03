import { useEffect, useState } from "react";
import axios from "axios";
import "./ManageStudents.css";

export default function ManageStudents() {
  const [students, setStudents] = useState([]);
  const [form, setForm] = useState({ userId: "", program: "", year: "" });
  const [editingId, setEditingId] = useState(null);
  const token = localStorage.getItem("token");

  const fetchStudents = async () => {
    try {
      const res = await axios.get("http://localhost:5000/api/student", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setStudents(res.data);
    } catch (err) {
      console.error("Error fetching students", err);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        await axios.put(`http://localhost:5000/api/student/${editingId}`, form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      } else {
        await axios.post("http://localhost:5000/api/student", form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      }
      setForm({ userId: "", program: "", year: "" });
      setEditingId(null);
      fetchStudents();
    } catch (err) {
      console.error("Error submitting form", err);
    }
  };

  const handleEdit = (student) => {
    setForm({ userId: student.userId, program: student.program, year: student.year });
    setEditingId(student.studentId);
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(`http://localhost:5000/api/student/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchStudents();
    } catch (err) {
      console.error("Error deleting student", err);
    }
  };

  return (
    <div className="manage-students">
      <h2>Manage Students</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="User ID"
          value={form.userId}
          onChange={(e) => setForm({ ...form, userId: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Program"
          value={form.program}
          onChange={(e) => setForm({ ...form, program: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Year"
          value={form.year}
          onChange={(e) => setForm({ ...form, year: e.target.value })}
          required
        />
        <button type="submit">{editingId ? "Update" : "Add"} Student</button>
      </form>

      <table>
        <thead>
          <tr>
            <th>Student ID</th>
            <th>User ID</th>
            <th>Program</th>
            <th>Year</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {students.map((s) => (
            <tr key={s.studentId}>
              <td>{s.studentId}</td>
              <td>{s.userId}</td>
              <td>{s.program}</td>
              <td>{s.year}</td>
              <td>
                <button onClick={() => handleEdit(s)}>Edit</button>
                <button onClick={() => handleDelete(s.studentId)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}