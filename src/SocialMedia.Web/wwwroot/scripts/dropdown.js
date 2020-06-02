const ToggleDropdown = (e) => {
	e.preventDefault();

	let container = e.target.parentNode;
	if (container.classList.contains("sm-dropdown-trigger")) {
		container = container.parentNode;
	}

	const dropdownContent = container.querySelector(".sm-dropdown-content");
	if (!dropdownContent) {
		return;
	}

	if (dropdownContent.style.display === "none") {
		dropdownContent.style.display = "block";
	} else {
		dropdownContent.style.display = "none";
	}
};

const InitialiseDropdowns = () => {
	const dropdowns = document.querySelectorAll(".sm-dropdown");
	const count = dropdowns.length;

	for (let i = 0; i < count; i++) {
		const dropdown = dropdowns[i];
		const trigger = dropdown.querySelector(".sm-dropdown-trigger");
		if (!trigger) {
			return;
		}

		trigger.addEventListener("click", ToggleDropdown);
	}
};

export default InitialiseDropdowns;
