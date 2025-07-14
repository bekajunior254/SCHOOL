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
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  // ✅ Fetch unassigned users (with Authorization header)
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const token = localStorage.getItem("token");
        const res = await axios.get("http://localhost:5000/api/admin/unassigned-users", {
          headers: { Authorization: `Bearer ${token}` }
        });
        setUsers(res.data);
      } catch (err) {
        console.error("Could not fetch users", err);
        setError("Failed to load users. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  // Handle input changes
  const handleChange = (e) => {
    setForm((prev) => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  // ✅ Submit form to assign role + identifier (with token)
  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage("");
    setError("");

    if (!form.userId || !form.identifier || !form.role) {
      setError("Please complete all fields.");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      await axios.post("http://localhost:5000/api/admin/assign-role-identifier", form, {
        headers: { Authorization: `Bearer ${token}` }
      });

      setMessage("✅ Role and identifier assigned successfully!");
      setForm({ userId: "", role: "Student", identifier: "" });

      // Remove user from list after assignment
      setUsers((prev) => prev.filter((u) => u.id !== form.userId));
    } catch (err) {
      console.error("Assign role error", err);
      setError("❌ Failed to assign role. Check the identifier or try again.");
    }
  };

  return (
    <div className="assign-role-container">
      <h2>Assign Role and Identifier</h2>

      {loading ? (
        <p>Loading users...</p>
      ) : users.length === 0 ? (
        <p>All users have been assigned roles.</p>
      ) : (
        <form onSubmit={handleSubmit} className="assign-role-form">
          <label>User:</label>
          <select name="userId" value={form.userId} onChange={handleChange} required>
            <option value="">-- Select User --</option>
            {users.map((user) => (
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
            placeholder="e.g. ADM001, TCH005"
            value={form.identifier}
            onChange={handleChange}
            required
          />

          <button type="submit">Assign</button>

          {message && <p className="success-message">{message}</p>}
          {error && <p className="error-message">{error}</p>}
        </form>
      )}
    </div>
  );
}
