import { useEffect, useState } from "react";
import axios from "axios";
import "./ManageTeachers.css";

export default function ManageTeachers() {
  const [teachers, setTeachers] = useState([]);
  const [form, setForm] = useState({ firstName: "", lastName: "", email: "" });
  const [editId, setEditId] = useState(null);

  const token = localStorage.getItem("token");

  const fetchTeachers = async () => {
    try {
      const res = await axios.get("http://localhost:5000/api/admin/teachers", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setTeachers(res.data);
    } catch (err) {
      console.error("Error fetching teachers", err);
    }
  };

  useEffect(() => {
    fetchTeachers();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      if (editId) {
        await axios.put(
          `http://localhost:5000/api/admin/teachers/${editId}`,
          form,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      } else {
        await axios.post("http://localhost:5000/api/admin/teachers", form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      }

      setForm({ firstName: "", lastName: "", email: "" });
      setEditId(null);
      fetchTeachers();
    } catch (err) {
      console.error("Error saving teacher", err);
    }
  };

  const handleEdit = (teacher) => {
    setForm({
      firstName: teacher.firstName,
      lastName: teacher.lastName,
      email: teacher.email,
    });
    setEditId(teacher.id);
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(`http://localhost:5000/api/admin/teachers/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchTeachers();
    } catch (err) {
      console.error("Error deleting teacher", err);
    }
  };

  return (
    <div className="manage-teachers-container">
      <h2>Manage Teachers</h2>

      <form onSubmit={handleSubmit} className="teacher-form">
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
          type="email"
          placeholder="Email"
          value={form.email}
          onChange={(e) => setForm({ ...form, email: e.target.value })}
          required
        />
        <button type="submit">{editId ? "Update" : "Add"} Teacher</button>
      </form>

      <ul>
        {teachers.map((teacher) => (
          <li key={teacher.id}>
            {teacher.firstName} {teacher.lastName} - {teacher.email}
            <div className="teacher-actions">
              <button onClick={() => handleEdit(teacher)}>Edit</button>
              <button className="delete-btn" onClick={() => handleDelete(teacher.id)}>
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
