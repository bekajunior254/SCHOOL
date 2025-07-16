import { useState, useEffect } from "react";
import axios from "axios";
import "./ManageCourses.css";

export default function ManageCourses() {
  const [courses, setCourses] = useState([]);
  const [form, setForm] = useState({ name: "", code: "", description: "" });
  const [editId, setEditId] = useState(null);

  const token = localStorage.getItem("token");

  const fetchCourses = async () => {
    try {
      const res = await axios.get("http://localhost:5000/api/courses", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setCourses(res.data);
    } catch (err) {
      console.error("Failed to fetch courses", err);
    }
  };

  useEffect(() => {
    fetchCourses();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editId) {
        await axios.put(`http://localhost:5000/api/courses/${editId}`, form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      } else {
        await axios.post("http://localhost:5000/api/courses", form, {
          headers: { Authorization: `Bearer ${token}` },
        });
      }

      setForm({ name: "", code: "", description: "" });
      setEditId(null);
      fetchCourses();
    } catch (err) {
      console.error("Failed to save course", err);
    }
  };

  const handleEdit = (course) => {
    setForm({
      name: course.name,
      code: course.code,
      description: course.description,
    });
    setEditId(course.id);
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(`http://localhost:5000/api/courses/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchCourses();
    } catch (err) {
      console.error("Failed to delete course", err);
    }
  };

  return (
    <div className="manage-courses">
      <h2>Manage Courses</h2>

      <form className="course-form" onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Course Name"
          value={form.name}
          onChange={(e) => setForm({ ...form, name: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Course Code"
          value={form.code}
          onChange={(e) => setForm({ ...form, code: e.target.value })}
          required
        />
        <input
          type="text"
          placeholder="Description"
          value={form.description}
          onChange={(e) => setForm({ ...form, description: e.target.value })}
        />
        <button type="submit">{editId ? "Update Course" : "Add Course"}</button>
      </form>

      <table className="courses-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Code</th>
            <th>Description</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {courses.map((course) => (
            <tr key={course.id}>
              <td>{course.name}</td>
              <td>{course.code}</td>
              <td>{course.description}</td>
              <td>
                <button onClick={() => handleEdit(course)}>Edit</button>
                <button className="delete-btn" onClick={() => handleDelete(course.id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
