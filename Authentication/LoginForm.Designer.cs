namespace Owcounter
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtUsername = new System.Windows.Forms.TextBox();
            txtPassword = new System.Windows.Forms.TextBox();
            btnLogin = new System.Windows.Forms.Button();
            lblUsername = new System.Windows.Forms.Label();
            lblPassword = new System.Windows.Forms.Label();
            btnSignUp = new System.Windows.Forms.Button();
            lblInstructions = new System.Windows.Forms.Label();
            chkShowPassword = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new System.Drawing.Point(93, 58);
            txtUsername.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(233, 23);
            txtUsername.TabIndex = 1;
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(93, 88);
            txtPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '•';
            txtPassword.Size = new System.Drawing.Size(233, 23);
            txtPassword.TabIndex = 2;
            // 
            // btnLogin
            // 
            btnLogin.Location = new System.Drawing.Point(93, 148);
            btnLogin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(88, 27);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new System.Drawing.Point(14, 61);
            lblUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(63, 15);
            lblUsername.TabIndex = 7;
            lblUsername.Text = "Username:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(14, 91);
            lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(60, 15);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password:";
            // 
            // btnSignUp
            // 
            btnSignUp.Location = new System.Drawing.Point(192, 148);
            btnSignUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSignUp.Name = "btnSignUp";
            btnSignUp.Size = new System.Drawing.Size(88, 27);
            btnSignUp.TabIndex = 5;
            btnSignUp.Text = "Sign Up";
            btnSignUp.UseVisualStyleBackColor = true;
            btnSignUp.Click += btnSignUp_Click;
            // 
            // lblInstructions
            // 
            lblInstructions.AutoSize = true;
            lblInstructions.Location = new System.Drawing.Point(14, 15);
            lblInstructions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInstructions.Name = "lblInstructions";
            lblInstructions.Size = new System.Drawing.Size(294, 30);
            lblInstructions.TabIndex = 2;
            lblInstructions.Text = "Please use your OWCOUNTER account credentials.\nIf you don't have an account, click the Sign Up button.";
            // 
            // chkShowPassword
            // 
            chkShowPassword.AutoSize = true;
            chkShowPassword.Location = new System.Drawing.Point(93, 118);
            chkShowPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Size = new System.Drawing.Size(108, 19);
            chkShowPassword.TabIndex = 3;
            chkShowPassword.Text = "Show Password";
            chkShowPassword.UseVisualStyleBackColor = true;
            chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(343, 190);
            Controls.Add(chkShowPassword);
            Controls.Add(btnSignUp);
            Controls.Add(lblInstructions);
            Controls.Add(btnLogin);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "LoginForm";
            Text = "OWCOUNTER Login";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnSignUp;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.CheckBox chkShowPassword;
    }
}