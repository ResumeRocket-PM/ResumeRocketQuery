import os
import random
import string
import subprocess
from flask import Flask, request, send_file, jsonify

app = Flask(__name__)

# Function to generate a random string for the filename
def random_filename(length=8):
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

# Route to handle PDF upload and conversion
@app.route('/convert', methods=['POST'])
def convert_pdf():
    if 'file' not in request.files:
        return jsonify({'error': 'No file part in the request'}), 400

    file = request.files['file']

    if file.filename == '':
        return jsonify({'error': 'No file selected for uploading'}), 400

    if file and file.filename.endswith('.pdf'):
        # Generate a random filename
        random_name = random_filename()
        input_pdf_path = f'/tmp/{random_name}.pdf'
        output_html_path = f'{random_name}.html'

        # Save the uploaded PDF to the /tmp directory
        file.save(input_pdf_path)

        # Run the pdf2htmlEX command
        try:
            result = subprocess.run(
                ['pdf2htmlEX', '--zoom', '1.3', input_pdf_path],
                check=True,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE
            )
            print(result.stdout.decode())  # Log standard output
        except subprocess.CalledProcessError as e:
            print(f"Error during PDF conversion: {e.stderr.decode()}")  # Log error output
            return jsonify({'error': 'Error during PDF conversion'}), 500

        # Debugging: Check if the input and output files exist
        if os.path.exists(input_pdf_path):
            print(f"Input PDF exists at: {input_pdf_path}")
        else:
            print(f"Input PDF not found at: {input_pdf_path}")

        if os.path.exists(output_html_path):
            print(f"Output HTML exists at: {output_html_path}")
            return send_file(output_html_path, as_attachment=True)
        else:
            return jsonify({'error': 'Converted HTML file not found'}), 500

    return jsonify({'error': 'Invalid file format. Only PDF allowed.'}), 400

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5010)
