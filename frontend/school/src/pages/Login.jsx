import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError(""); 

    try {
      const res = await axios.post("http://localhost:5000/api/auth/login", {
        username,
        password,
      });

      
      console.log("Login response:", res.data);

      // Store token & user info
      localStorage.setItem("token", res.data.token);
      localStorage.setItem("role", res.data.role);
      localStorage.setItem("username", res.data.username);

      // Navigate based on role
      const role = res.data.role;
      switch (role) {
        case "Admin":
          navigate("/admin");
          break;
        case "Teacher":
          navigate("/teacher");
          break;
        case "Student":
          navigate("/student");
          break;
        case "Parent":
          navigate("/parent");
          break;
        default:
          setError("No dashboard available for this role");
      }
    } catch (err) {
      console.error("Login error:", err?.response || err);
      if (err?.response?.status === 403) {
        setError("No role assigned. Contact admin.");
      } else {
        setError("Invalid username or password");
      }
    }
  };

  return (
    <div className="login-page">
      <h2>Login</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <form onSubmit={handleLogin}>
        <input
          type="text"
          placeholder="Username"
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Password"
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Login</button>
      </form>
    </div>
  );
}
