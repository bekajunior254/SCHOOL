import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import axios from "axios";
import "./ManageStudents.css";

export default function ManageStudents() {
  const [students, setStudents] = useState([]);
  const [form, setForm] = useState({ firstName: "", lastName: "", program: "", year: "" });
  const [editId, setEditId] = useState(null);
  const location = useLocation();
  const refreshStats = location.state?.refreshStats;

  const fetchStudents = async () => {
    try {
      const token = localStorage.getItem("token");
      const res = await axios.get("http://localhost:5000/api/student", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setStudents(res.data);
    } catch (err) {
      console.error("Failed to fetch students", err);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const token = localStorage.getItem("token");

    try {
      if (editId) {
        // Update existing student
        await axios.put(`http://localhost:5000/api/student/${editId}`, form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      } else {
        // Create new student
        await axios.post("http://localhost:5000/api/student", form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      }

      setForm({ firstName: "", lastName: "", program: "", year: "" });
      setEditId(null);
      fetchStudents();
      refreshStats?.();
    } catch (err) {
      console.error("Failed to save student", err);
    }
  };

  const handleEdit = (student) => {
    setForm({
      firstName: student.firstName,
      lastName: student.lastName,
      program: student.program,
      year: student.year,
    });
    setEditId(student.studentId);
  };

  const handleDelete = async (id) => {
    const token = localStorage.getItem("token");
    try {
      await axios.delete(`http://localhost:5000/api/student/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchStudents();
      refreshStats?.();
    } catch (err) {
      console.error("Failed to delete student", err);
    }
  };

  return (
    <div className="manage-students">
      <h2>Manage Students</h2>

      <form onSubmit={handleSubmit} className="student-form">
        <input
          type="text"
          placeholder="First Name"
          value={form.firstName}
          onChange={(e) => setForm({ ...form, firstName: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Last Name"
          value={form.lastName}
          onChange={(e) => setForm({ ...form, lastName: e.target.value })}
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
        <button type="submit">{editId ? "Update" : "Add"} Student</button>
      </form>

      <table className="student-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Program</th>
            <th>Year</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {students.map((s) => (
            <tr key={s.studentId}>
              <td>{s.firstName} {s.lastName}</td>
              <td>{s.program}</td>
              <td>{s.year}</td>
              <td>
                <button onClick={() => handleEdit(s)}>Edit</button>
                <button onClick={() => handleDelete(s.studentId)} className="delete-btn">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}