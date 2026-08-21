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
    ssn: "",
  });
  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    if (errors[name]) {
      setErrors({ ...errors, [name]: "" });
    }
  };

  const validateForm = () => {
    const nextErrors = {};
    const requiredFields = ["firstName", "lastName", "state", "ssn"];

    requiredFields.forEach((field) => {
      if (!formData[field].trim()) nextErrors[field] = "This field is required.";
    });
    if (!formData.requestedAmount || Number(formData.requestedAmount) <= 0) {
      nextErrors.requestedAmount = "Enter an amount greater than $0.";
    }
    if (formData.ssn && formData.ssn.trim().length < 6) {
      nextErrors.ssn = "Enter at least 6 characters.";
    }

    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    setIsSubmitting(true);
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
        router.push(`/denied?reason=${encodeURIComponent(result.reason || "Application did not meet the approval criteria.")}`);
      }
    } catch {
      setErrors({ form: "We could not submit your application. Check the connection and try again." });
      setIsSubmitting(false);
    }
  };

  const fieldError = (field) => errors[field] && <span className="field-error">{errors[field]}</span>;

  return (
    <main className="application-shell">
      <div className="application-layout">
        <aside className="application-intro">
          <div className="brand-mark" aria-hidden="true">N</div>
          <p className="eyebrow">NIURO / BUSINESS LENDING</p>
          <h1>Move your next idea forward.</h1>
          <p className="intro-copy">A straightforward application for the capital your business needs to keep growing.</p>
          <div className="trust-note">
            <span className="trust-icon" aria-hidden="true">✓</span>
            <span>Your information is handled securely.</span>
          </div>
        </aside>

        <form onSubmit={handleSubmit} className="application-form" noValidate>
          <div className="form-heading">
            <div>
              <p className="eyebrow">STEP 1 OF 1</p>
              <h2>Tell us about you</h2>
            </div>
            <span className="required-note">* Required</span>
          </div>
          <p className="form-description">It takes about 3 minutes. We will use this information to review your request.</p>

          {errors.form && <div className="form-error" role="alert">{errors.form}</div>}

          <fieldset>
            <legend>Personal details</legend>
            <div className="field-grid">
              <label className="field-group">First name <span>*</span>
                <input name="firstName" value={formData.firstName} onChange={handleChange} aria-invalid={Boolean(errors.firstName)} autoComplete="given-name" />
                {fieldError("firstName")}
              </label>
              <label className="field-group">Last name <span>*</span>
                <input name="lastName" value={formData.lastName} onChange={handleChange} aria-invalid={Boolean(errors.lastName)} autoComplete="family-name" />
                {fieldError("lastName")}
              </label>
            </div>
            <label className="field-group">Address
              <input name="address" value={formData.address} onChange={handleChange} autoComplete="street-address" />
            </label>
            <div className="field-grid">
              <label className="field-group">State <span>*</span>
                <input name="state" value={formData.state} onChange={handleChange} aria-invalid={Boolean(errors.state)} autoComplete="address-level1" />
                {fieldError("state")}
              </label>
              <label className="field-group">Company name
                <input name="companyName" value={formData.companyName} onChange={handleChange} autoComplete="organization" />
              </label>
            </div>
          </fieldset>

          <fieldset>
            <legend>Loan details</legend>
            <div className="field-grid">
              <label className="field-group">Requested amount <span>*</span>
                <div className="input-prefix"><span>$</span><input name="requestedAmount" type="number" min="1" value={formData.requestedAmount} onChange={handleChange} aria-invalid={Boolean(errors.requestedAmount)} placeholder="0" /></div>
                {fieldError("requestedAmount")}
              </label>
              <label className="field-group">SSN <span>*</span>
                <input name="ssn" value={formData.ssn} onChange={handleChange} aria-invalid={Boolean(errors.ssn)} autoComplete="off" />
                {fieldError("ssn")}
              </label>
            </div>
          </fieldset>

          <button type="submit" className="submit-button" disabled={isSubmitting}>
            {isSubmitting ? "Reviewing application..." : "Submit application"}
            {!isSubmitting && <span aria-hidden="true">→</span>}
          </button>
          <p className="form-footnote">By submitting, you agree that we may review this information for lending purposes.</p>
        </form>
      </div>
    </main>
  );
}
