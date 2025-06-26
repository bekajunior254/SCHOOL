import { useState, useEffect } from "react";
import axios from "axios";
import "./AssignRolePage.css";

export default function AssignRoleForm() {
  const [form, setForm] = useState({
    userId: "",
    role: "Student",
    identifier: ""
  });
  const [users, setUsers] = useState([]);
  const [message, setMessage] = useState("");

  // Fetch all users without roles (or with placeholder usernames)
  useEffect(() => {
    axios.get("http://localhost:5000/api/admin/unassigned-users") // Optional endpoint if available
      .then(res => setUsers(res.data))
      .catch(err => console.error("Could not fetch users", err));
  }, []);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await axios.post("http://localhost:5000/api/auth/assign-role", form);
      setMessage("Role and identifier assigned successfully!");
    } catch (err) {
      console.error(err);
      setMessage("Failed to assign role.");
    }
  };

  return (
    <div className="assign-role-container">
      <h2>Assign Role and Identifier</h2>
      <form onSubmit={handleSubmit} className="assign-role-form">
        <label>User:</label>
        <select name="userId" value={form.userId} onChange={handleChange} required>
          <option value="">-- Select User --</option>
          {users.map(user => (
            <option key={user.id} value={user.id}>
              {user.firstName} {user.lastName} ({user.email})
            </option>
          ))}
        </select>

        <label>Role:</label>
        <select name="role" value={form.role} onChange={handleChange}>
          <option value="Student">Student</option>
          <option value="Teacher">Teacher</option>
          <option value="Parent">Parent</option>
        </select>

        <label>Identifier:</label>
        <input
          type="text"
          name="identifier"
          value={form.identifier}
          onChange={handleChange}
          required
        />

        <button type="submit">Assign</button>
        {message && <p className="status-message">{message}</p>}
      </form>
    </div>
  );
}
