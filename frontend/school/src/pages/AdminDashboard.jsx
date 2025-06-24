import React from "react";
import { useNavigate } from "react-router-dom";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  const navigate = useNavigate();

  return (
    <div className="admin-dashboard">
      <h1>Welcome, Admin</h1>

      <div className="admin-actions">
        <button onClick={() => navigate("/admin/manage-users")}>Manage Users</button>
        <button onClick={() => navigate("/admin/manage-students")}>Manage Students</button>
        <button onClick={() => navigate("/admin/manage-teachers")}>Manage Teachers</button>
        <button onClick={() => navigate("/admin/create-course")}>Create Course</button>
        <button onClick={() => navigate("/admin/assign-students")}>Assign Students to Courses</button>
      </div>
    </div>
  );
}
