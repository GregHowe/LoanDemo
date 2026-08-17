"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";

export default function ApplicationForm() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    address: "",
    state: "",
    companyName: "",
    requestedAmount: "",
    ssn: ""
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const validateForm = () => {
    if (!formData.firstName || !formData.lastName || !formData.state || !formData.ssn) {
      alert("Please fill in all required fields.");
      return false;
    }
    if (!formData.requestedAmount || Number(formData.requestedAmount) <= 0) {
      alert("The requested amount must be greater than 0.");
      return false;
    }
    if (formData.ssn.length < 6) {
      alert("SSN must be at least 6 characters long.");
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      const response = await fetch("https://localhost:7228/api/Applications", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });

      const result = await response.json();

      if (result.approved) {
        router.push("/approved");
      } else {
        router.push(`/denied?reason=${result.reason}`);
      }
    } catch (error) {
      alert("Error submitting the application. Please try again.");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-black px-4 py-8">
      <form onSubmit={handleSubmit} className="w-full max-w-md p-6 bg-white shadow rounded text-black">
        <h1 className="text-xl font-bold mb-4 text-black">Loan Application</h1>

      <label className="block mb-2 text-black">First Name</label>
      <input
        name="firstName"
        value={formData.firstName}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-4 text-black placeholder:text-gray-500"
      />

      <label className="block mb-2 text-black">Last Name</label>
      <input
        name="lastName"
        value={formData.lastName}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-4 text-black placeholder:text-gray-500"
      />

      <label className="block mb-2 text-black">Address</label>
      <input
        name="address"
        value={formData.address}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-4 text-black placeholder:text-gray-500"
      />

      <label className="block mb-2 text-black">State</label>
      <input
        name="state"
        value={formData.state}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-4 text-black placeholder:text-gray-500"
      />

      <label className="block mb-2 text-black">Company Name</label>
      <input
        name="companyName"
        value={formData.companyName}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-4 text-black placeholder:text-gray-500"
      />

      <label className="block mb-2 text-black">Requested Amount</label>
      <input
        name="requestedAmount"
        type="number"
        value={formData.requestedAmount}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-4 text-black placeholder:text-gray-500"
      />

      <label className="block mb-2 text-black">SSN</label>
      <input
        name="ssn"
        value={formData.ssn}
        onChange={handleChange}
        className="border p-2 rounded w-full mb-6 text-black placeholder:text-gray-500"
      />

        <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded w-full">
          Submit Application
        </button>
      </form>
    </div>
  );
}
